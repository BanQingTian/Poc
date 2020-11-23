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
Object.defineProperty(exports, "__esModule", { value: true });
const schema_1 = require("@colyseus/schema");
const Room_1 = require("../Room");
/**
 * Create another context to avoid these types from being in the user's global `Context`
 */
const context = new schema_1.Context();
class Player extends schema_1.Schema {
}
schema_1.defineTypes(Player, {
    connected: 'boolean',
    sessionId: 'string',
}, context);
class State extends schema_1.Schema {
    constructor() {
        super(...arguments);
        this.players = new schema_1.MapSchema();
    }
}
schema_1.defineTypes(State, {
    players: { map: Player },
}, context);
/**
 * client.joinOrCreate("relayroom", {
 *   maxClients: 10,
 *   allowReconnectionTime: 20
 * });
 */
class RelayRoom extends Room_1.Room {
    constructor() {
        super(...arguments);
        this.allowReconnectionTime = 0;
    }
    onCreate(options) {
        this.setState(new State());
        if (options.maxClients) {
            this.maxClients = options.maxClients;
        }
        if (options.allowReconnectionTime) {
            this.allowReconnectionTime = Math.min(options.allowReconnectionTime, 40);
        }
        if (options.metadata) {
            this.setMetadata(options.metadata);
        }
    }
    onJoin(client, options) {
        const player = new Player();
        player.connected = true;
        player.sessionId = client.sessionId;
        this.state.players[client.sessionId] = player;
    }
    onMessage(client, message) {
        /**
         * append `sessionId` into the message for broadcast.
         */
        if (typeof (message) === 'object' && !Array.isArray(message)) {
            message.sessionId = client.sessionId;
        }
        this.broadcast(message, { except: client });
    }
    onLeave(client, consented) {
        return __awaiter(this, void 0, void 0, function* () {
            if (this.allowReconnectionTime > 0) {
                this.state.players[client.sessionId].connected = false;
                try {
                    if (consented) {
                        throw new Error('consented leave');
                    }
                    yield this.allowReconnection(client, this.allowReconnectionTime);
                    this.state.players[client.sessionId].connected = true;
                }
                catch (e) {
                    delete this.state.players[client.sessionId];
                }
            }
        });
    }
}
exports.RelayRoom = RelayRoom;
