"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const notepack_io_1 = __importDefault(require("notepack.io"));
const schema_1 = require("@colyseus/schema");
const timer_1 = __importDefault(require("@gamestdio/timer"));
const events_1 = require("events");
const SchemaSerializer_1 = require("./serializer/SchemaSerializer");
const Protocol_1 = require("./Protocol");
const Utils_1 = require("./Utils");
const Debug_1 = require("./Debug");
const FossilDeltaSerializer_1 = require("./serializer/FossilDeltaSerializer");
const DEFAULT_PATCH_RATE = 1000 / 20; // 20fps (50ms)
const DEFAULT_SIMULATION_INTERVAL = 1000 / 60; // 60fps (16.66ms)
exports.DEFAULT_SEAT_RESERVATION_TIME = Number(process.env.COLYSEUS_SEAT_RESERVATION_TIME || 8);
var RoomInternalState;
(function (RoomInternalState) {
    RoomInternalState[RoomInternalState["CREATING"] = 0] = "CREATING";
    RoomInternalState[RoomInternalState["CREATED"] = 1] = "CREATED";
    RoomInternalState[RoomInternalState["DISCONNECTING"] = 2] = "DISCONNECTING";
})(RoomInternalState = exports.RoomInternalState || (exports.RoomInternalState = {}));
class Room extends events_1.EventEmitter {
    constructor(presence) {
        super();
        this.clock = new timer_1.default();
        this.maxClients = Infinity;
        this.patchRate = DEFAULT_PATCH_RATE;
        this.autoDispose = true;
        this.clients = [];
        /** @internal */
        this._internalState = RoomInternalState.CREATING;
        // seat reservation & reconnection
        this.seatReservationTime = exports.DEFAULT_SEAT_RESERVATION_TIME;
        this.reservedSeats = {};
        this.reservedSeatTimeouts = {};
        this.reconnections = {};
        this._serializer = new FossilDeltaSerializer_1.FossilDeltaSerializer();
        this._afterNextPatchBroadcasts = [];
        this._locked = false;
        this._lockedExplicitly = false;
        this._maxClientsReached = false;
        this.presence = presence;
        this.once('dispose', () => __awaiter(this, void 0, void 0, function* () {
            try {
                yield this._dispose();
            }
            catch (e) {
                Debug_1.debugAndPrintError(`onDispose error: ${(e && e.message || e || 'promise rejected')}`);
            }
            this.emit('disconnect');
        }));
        this.setPatchRate(this.patchRate);
    }
    get locked() {
        return this._locked;
    }
    onAuth(client, options, request) {
        return true;
    }
    hasReachedMaxClients() {
        return (this.clients.length + Object.keys(this.reservedSeats).length) >= this.maxClients;
    }
    setSeatReservationTime(seconds) {
        this.seatReservationTime = seconds;
        return this;
    }
    hasReservedSeat(sessionId) {
        return this.reservedSeats[sessionId] !== undefined;
    }
    setSimulationInterval(callback, delay = DEFAULT_SIMULATION_INTERVAL) {
        // clear previous interval in case called setSimulationInterval more than once
        if (this._simulationInterval) {
            clearInterval(this._simulationInterval);
        }
        this._simulationInterval = setInterval(() => {
            this.clock.tick();
            callback(this.clock.deltaTime);
        }, delay);
    }
    setPatchRate(milliseconds) {
        // clear previous interval in case called setPatchRate more than once
        if (this._patchInterval) {
            clearInterval(this._patchInterval);
            this._patchInterval = undefined;
        }
        if (milliseconds !== null && milliseconds !== 0) {
            this._patchInterval = setInterval(() => {
                this.broadcastPatch();
                this.broadcastAfterPatch();
            }, milliseconds);
        }
    }
    setState(newState) {
        this.clock.start();
        if ('_schema' in newState) {
            this._serializer = new SchemaSerializer_1.SchemaSerializer();
        }
        else {
            this._serializer = new FossilDeltaSerializer_1.FossilDeltaSerializer();
        }
        this._serializer.reset(newState);
        this.state = newState;
    }
    setMetadata(meta) {
        if (!this.listing.metadata) {
            this.listing.metadata = meta;
        }
        else {
            for (const field in meta) {
                if (!meta.hasOwnProperty(field)) {
                    continue;
                }
                this.listing.metadata[field] = meta[field];
            }
        }
        if (this._internalState === RoomInternalState.CREATED) {
            this.listing.save();
        }
    }
    setPrivate(bool = true) {
        return __awaiter(this, void 0, void 0, function* () {
            this.listing.private = bool;
            if (this._internalState === RoomInternalState.CREATED) {
                return yield this.listing.save();
            }
        });
    }
    get metadata() {
        return this.listing.metadata;
    }
    lock() {
        return __awaiter(this, arguments, void 0, function* () {
            // rooms locked internally aren't explicit locks.
            this._lockedExplicitly = (arguments[0] === undefined);
            // skip if already locked.
            if (this._locked) {
                return;
            }
            this.emit('lock');
            this._locked = true;
            return yield this.listing.updateOne({
                $set: { locked: this._locked },
            });
        });
    }
    unlock() {
        return __awaiter(this, arguments, void 0, function* () {
            // only internal usage passes arguments to this function.
            if (arguments[0] === undefined) {
                this._lockedExplicitly = false;
            }
            // skip if already locked
            if (!this._locked) {
                return;
            }
            this.emit('unlock');
            this._locked = false;
            return yield this.listing.updateOne({
                $set: { locked: this._locked },
            });
        });
    }
    send(client, message) {
        if (message instanceof schema_1.Schema) {
            Protocol_1.send.raw(client, [
                Protocol_1.Protocol.ROOM_DATA_SCHEMA,
                message.constructor._typeid,
                ...message.encodeAll(),
            ]);
        }
        else {
            Protocol_1.send[Protocol_1.Protocol.ROOM_DATA](client, message);
        }
    }
    broadcast(message, options = {}) {
        if (options.afterNextPatch) {
            delete options.afterNextPatch;
            this._afterNextPatchBroadcasts.push([message, options]);
            return true;
        }
        // no data given, try to broadcast patched state
        if (!message) {
            throw new Error('Room#broadcast: \'data\' is required to broadcast.');
        }
        if (message instanceof schema_1.Schema) {
            const typeId = message.constructor._typeid;
            const encodedMessage = Buffer.from([Protocol_1.Protocol.ROOM_DATA_SCHEMA, typeId, ...message.encodeAll()]);
            let numClients = this.clients.length;
            while (numClients--) {
                const client = this.clients[numClients];
                if (options.except !== client) {
                    Protocol_1.send.raw(client, encodedMessage);
                }
            }
        }
        else {
            // encode message with msgpack
            const encodedMessage = (!(message instanceof Buffer))
                ? Buffer.from([Protocol_1.Protocol.ROOM_DATA, ...notepack_io_1.default.encode(message)])
                : message;
            let numClients = this.clients.length;
            while (numClients--) {
                const client = this.clients[numClients];
                if (options.except !== client) {
                    Protocol_1.send.raw(client, encodedMessage);
                }
            }
        }
        return true;
    }
    disconnect() {
        this._internalState = RoomInternalState.DISCONNECTING;
        this.autoDispose = true;
        const delayedDisconnection = new Promise((resolve) => this.once('disconnect', () => resolve()));
        for (const reconnection of Object.values(this.reconnections)) {
            reconnection.reject();
        }
        let numClients = this.clients.length;
        if (numClients > 0) {
            // prevent new clients to join while this room is disconnecting.
            this.lock();
            // clients may have `async onLeave`, room will be disposed after they all run
            while (numClients--) {
                this._forciblyCloseClient(this.clients[numClients], Protocol_1.Protocol.WS_CLOSE_CONSENTED);
            }
        }
        else {
            // no clients connected, dispose immediately.
            this.emit('dispose');
        }
        return delayedDisconnection;
    }
    ['_onJoin'](client, req) {
        return __awaiter(this, void 0, void 0, function* () {
            const sessionId = client.sessionId;
            if (this.reservedSeatTimeouts[sessionId]) {
                clearTimeout(this.reservedSeatTimeouts[sessionId]);
                delete this.reservedSeatTimeouts[sessionId];
            }
            // clear auto-dispose timeout.
            if (this._autoDisposeTimeout) {
                clearTimeout(this._autoDisposeTimeout);
                this._autoDisposeTimeout = undefined;
            }
            // get seat reservation options and clear it
            const options = this.reservedSeats[sessionId];
            delete this.reservedSeats[sessionId];
            // bind clean-up callback when client connection closes
            client.once('close', this._onLeave.bind(this, client));
            client.state = Protocol_1.ClientState.JOINING;
            client._enqueuedMessages = [];
            this.clients.push(client);
            const reconnection = this.reconnections[sessionId];
            if (reconnection) {
                reconnection.resolve(client);
            }
            else {
                try {
                    client.auth = yield this.onAuth(client, options, req);
                    if (!client.auth) {
                        throw new Error('onAuth failed.');
                    }
                    if (this.onJoin) {
                        yield this.onJoin(client, options, client.auth);
                    }
                }
                catch (e) {
                    Utils_1.spliceOne(this.clients, this.clients.indexOf(client));
                    throw e;
                }
                finally {
                    // remove seat reservation
                    delete this.reservedSeats[sessionId];
                }
            }
            // emit 'join' to room handler
            this.emit('join', client);
            // allow client to send messages after onJoin has succeeded.
            client.on('message', this._onMessage.bind(this, client));
            // confirm room id that matches the room name requested to join
            Protocol_1.send[Protocol_1.Protocol.JOIN_ROOM](client, this._serializer.id, this._serializer.handshake && this._serializer.handshake());
        });
    }
    sendState(client) {
        Protocol_1.send[Protocol_1.Protocol.ROOM_STATE](client, this._serializer.getFullState(client));
    }
    broadcastPatch() {
        if (!this._simulationInterval) {
            this.clock.tick();
        }
        if (!this.state) {
            Debug_1.debugPatch('trying to broadcast null state. you should call #setState');
            return false;
        }
        return this._serializer.applyPatches(this.clients, this.state);
    }
    broadcastAfterPatch() {
        const length = this._afterNextPatchBroadcasts.length;
        if (length > 0) {
            for (let i = 0; i < length; i++) {
                this.broadcast.apply(this, this._afterNextPatchBroadcasts[i]);
            }
            // new messages may have been added in the meantime,
            // let's splice the ones that have been processed
            this._afterNextPatchBroadcasts.splice(0, length);
        }
    }
    allowReconnection(previousClient, seconds = Infinity) {
        if (this._internalState === RoomInternalState.DISCONNECTING) {
            this._disposeIfEmpty(); // gracefully shutting down
            throw new Error('disconnecting');
        }
        const sessionId = previousClient.sessionId;
        this._reserveSeat(sessionId, true, seconds, true);
        // keep reconnection reference in case the user reconnects into this room.
        const reconnection = new Utils_1.Deferred();
        this.reconnections[sessionId] = reconnection;
        if (seconds !== Infinity) {
            // expire seat reservation after timeout
            this.reservedSeatTimeouts[sessionId] = setTimeout(() => reconnection.reject(false), seconds * 1000);
        }
        const cleanup = () => {
            delete this.reservedSeats[sessionId];
            delete this.reconnections[sessionId];
            delete this.reservedSeatTimeouts[sessionId];
        };
        reconnection.
            then((newClient) => {
            newClient.auth = previousClient.auth;
            previousClient.state = Protocol_1.ClientState.RECONNECTED;
            clearTimeout(this.reservedSeatTimeouts[sessionId]);
            cleanup();
        }).
            catch(() => {
            cleanup();
            this._disposeIfEmpty();
        });
        return reconnection;
    }
    _reserveSeat(sessionId, joinOptions = true, seconds = this.seatReservationTime, allowReconnection = false) {
        return __awaiter(this, void 0, void 0, function* () {
            if (!allowReconnection && this.hasReachedMaxClients()) {
                return false;
            }
            this.reservedSeats[sessionId] = joinOptions;
            if (!allowReconnection) {
                yield this._incrementClientCount();
                this.reservedSeatTimeouts[sessionId] = setTimeout(() => __awaiter(this, void 0, void 0, function* () {
                    delete this.reservedSeats[sessionId];
                    delete this.reservedSeatTimeouts[sessionId];
                    yield this._decrementClientCount();
                }), seconds * 1000);
                this.resetAutoDisposeTimeout(seconds);
            }
            return true;
        });
    }
    resetAutoDisposeTimeout(timeoutInSeconds) {
        clearTimeout(this._autoDisposeTimeout);
        if (!this.autoDispose) {
            return;
        }
        this._autoDisposeTimeout = setTimeout(() => {
            this._autoDisposeTimeout = undefined;
            this._disposeIfEmpty();
        }, timeoutInSeconds * 1000);
    }
    _disposeIfEmpty() {
        const willDispose = (this.autoDispose &&
            this._autoDisposeTimeout === undefined &&
            this.clients.length === 0 &&
            Object.keys(this.reservedSeats).length === 0);
        if (willDispose) {
            this.emit('dispose');
        }
        return willDispose;
    }
    _dispose() {
        return __awaiter(this, void 0, void 0, function* () {
            let userReturnData;
            if (this.onDispose) {
                userReturnData = this.onDispose();
            }
            if (this._patchInterval) {
                clearInterval(this._patchInterval);
                this._patchInterval = undefined;
            }
            if (this._simulationInterval) {
                clearInterval(this._simulationInterval);
                this._simulationInterval = undefined;
            }
            if (this._autoDisposeTimeout) {
                clearInterval(this._autoDisposeTimeout);
                this._autoDisposeTimeout = undefined;
            }
            // clear all timeouts/intervals + force to stop ticking
            this.clock.clear();
            this.clock.stop();
            return yield (userReturnData || Promise.resolve());
        });
    }
    _onMessage(client, message) {
        message = Protocol_1.decode(message);
        if (!message) {
            Debug_1.debugAndPrintError(`${this.roomName} (${this.roomId}), couldn't decode message: ${message}`);
            return;
        }
        if (message[0] === Protocol_1.Protocol.ROOM_DATA) {
            this.onMessage(client, message[1]);
        }
        else if (message[0] === Protocol_1.Protocol.JOIN_ROOM) {
            // join room has been acknowledged by the client
            client.state = Protocol_1.ClientState.JOINED;
            // send current state when new client joins the room
            if (this.state) {
                this.sendState(client);
            }
            // dequeue messages sent before client has joined effectively (on user-defined `onJoin`)
            if (client._enqueuedMessages.length > 0) {
                client._enqueuedMessages.forEach((bytes) => Protocol_1.send.raw(client, bytes));
            }
            delete client._enqueuedMessages;
        }
        else if (message[0] === Protocol_1.Protocol.LEAVE_ROOM) {
            this._forciblyCloseClient(client, Protocol_1.Protocol.WS_CLOSE_CONSENTED);
        }
        else {
            this.onMessage(client, message);
        }
    }
    _forciblyCloseClient(client, closeCode) {
        // stop receiving messages from this client
        client.removeAllListeners('message');
        // prevent "onLeave" from being called twice if player asks to leave
        const closeListeners = client.listeners('close');
        if (closeListeners.length >= 2) {
            client.removeListener('close', closeListeners[1]);
        }
        // only effectively close connection when "onLeave" is fulfilled
        this._onLeave(client, closeCode).then(() => client.close(Protocol_1.Protocol.WS_CLOSE_NORMAL));
    }
    _onLeave(client, code) {
        return __awaiter(this, void 0, void 0, function* () {
            const success = Utils_1.spliceOne(this.clients, this.clients.indexOf(client));
            // call abstract 'onLeave' method only if the client has been successfully accepted.
            if (success) {
                if (this.onLeave) {
                    try {
                        yield this.onLeave(client, (code === Protocol_1.Protocol.WS_CLOSE_CONSENTED));
                    }
                    catch (e) {
                        Debug_1.debugAndPrintError(`onLeave error: ${(e && e.message || e || 'promise rejected')}`);
                    }
                }
                if (client.state !== Protocol_1.ClientState.RECONNECTED) {
                    this.emit('leave', client);
                }
            }
            // skip next checks if client has reconnected successfully (through `allowReconnection()`)
            if (client.state === Protocol_1.ClientState.RECONNECTED) {
                return;
            }
            // try to dispose immediatelly if client reconnection isn't set up.
            yield this._decrementClientCount();
        });
    }
    _incrementClientCount() {
        return __awaiter(this, void 0, void 0, function* () {
            // lock automatically when maxClients is reached
            if (!this._locked && this.hasReachedMaxClients()) {
                this._maxClientsReached = true;
                this.lock.call(this, true);
            }
            yield this.listing.updateOne({
                $inc: { clients: 1 },
                $set: { locked: this._locked },
            });
        });
    }
    _decrementClientCount() {
        return __awaiter(this, void 0, void 0, function* () {
            const willDispose = this._disposeIfEmpty();
            // unlock if room is available for new connections
            if (!willDispose) {
                if (this._maxClientsReached && !this._lockedExplicitly) {
                    this._maxClientsReached = false;
                    this.unlock.call(this, true);
                }
                // update room listing cache
                yield this.listing.updateOne({
                    $inc: { clients: -1 },
                    $set: { locked: this._locked },
                });
            }
        });
    }
}
exports.Room = Room;
