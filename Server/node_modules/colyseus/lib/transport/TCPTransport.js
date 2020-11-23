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
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result["default"] = mod;
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
const net = __importStar(require("net"));
const __1 = require("../");
const Protocol_1 = require("../Protocol");
const matchMaker = __importStar(require("./../MatchMaker"));
const Transport_1 = require("./Transport");
const Debug_1 = require("./../Debug");
/**
 * TODO:
 * TCPTransport is not working.
 * It was meant to be used for https://github.com/colyseus/colyseus-gml
 */
class TCPTransport extends Transport_1.Transport {
    constructor(options = {}) {
        super();
        this.server = net.createServer();
        this.server.on('connection', this.onConnection);
    }
    listen(port, hostname, backlog, listeningListener) {
        this.server.listen(port, hostname, backlog, listeningListener);
        return this;
    }
    shutdown() {
        this.server.close();
    }
    onConnection(client) {
        // compatibility with ws / uws
        const upgradeReq = {};
        // set client id
        client.id = upgradeReq.colyseusid || __1.generateId();
        client.pingCount = 0;
        // set client options
        client.options = upgradeReq.options;
        client.auth = upgradeReq.auth;
        // prevent server crashes if a single client had unexpected error
        client.on('error', (err) => Debug_1.debugError(err.message + '\n' + err.stack));
        // client.on('pong', heartbeat);
        client.on('data', (data) => this.onMessage(client, Protocol_1.decode(data)));
    }
    onMessage(client, message) {
        return __awaiter(this, void 0, void 0, function* () {
            console.log('RECEIVED:', message);
            if (message[0] === Protocol_1.Protocol.JOIN_ROOM) {
                const roomId = message[1];
                const sessionId = message[2];
                client.id = sessionId;
                client.sessionId = sessionId;
                console.log('EFFECTIVELY CONNECT INTO ROOM', roomId, client.id, client.options);
                client.removeAllListeners('data');
                // forward as 'message' all 'data' messages
                client.on('data', (data) => client.emit('message', data));
                const room = matchMaker.getRoomById(roomId);
                try {
                    if (!room || !room.hasReservedSeat(sessionId)) {
                        throw new Error('seat reservation expired.');
                    }
                    yield room._onJoin(client);
                }
                catch (e) {
                    Debug_1.debugAndPrintError(e);
                    Protocol_1.send[Protocol_1.Protocol.JOIN_ERROR](client, (e && e.message) || '');
                    client.close(Protocol_1.Protocol.WS_CLOSE_WITH_ERROR);
                }
            }
        });
    }
}
exports.TCPTransport = TCPTransport;
