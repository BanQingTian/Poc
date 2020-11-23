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
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result["default"] = mod;
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
const querystring_1 = __importDefault(require("querystring"));
const url_1 = __importDefault(require("url"));
const __1 = require("..");
const matchMaker = __importStar(require("../MatchMaker"));
const Protocol_1 = require("../Protocol");
const Transport_1 = require("./Transport");
const Debug_1 = require("./../Debug");
function noop() { }
function heartbeat() { this.pingCount = 0; }
class WebSocketTransport extends Transport_1.Transport {
    constructor(options = {}, engine) {
        super();
        // disable per-message deflate
        options.perMessageDeflate = false;
        if (options.pingTimeout !== undefined) {
            console.warn('"pingTimeout" is deprecated. Use "pingInterval" instead.');
            options.pingInterval = options.pingTimeout;
        }
        if (options.pingCountMax !== undefined) {
            console.warn('"pingCountMax" is deprecated. Use "pingMaxRetries" instead.');
            options.pingMaxRetries = options.pingCountMax;
        }
        this.pingIntervalMS = (options.pingInterval !== undefined)
            ? options.pingInterval
            : 1500;
        this.pingMaxRetries = (options.pingMaxRetries !== undefined)
            ? options.pingMaxRetries
            : 2;
        this.wss = new engine(options);
        this.wss.on('connection', this.onConnection);
        this.server = options.server;
        if (this.pingIntervalMS > 0 && this.pingMaxRetries > 0) {
            this.autoTerminateUnresponsiveClients(this.pingIntervalMS, this.pingMaxRetries);
        }
    }
    listen(port, hostname, backlog, listeningListener) {
        this.server.listen(port, hostname, backlog, listeningListener);
        return this;
    }
    shutdown() {
        clearInterval(this.pingInterval);
        this.wss.close();
        this.server.close();
    }
    autoTerminateUnresponsiveClients(pingInterval, pingMaxRetries) {
        // interval to detect broken connections
        this.pingInterval = setInterval(() => {
            this.wss.clients.forEach((client) => {
                //
                // if client hasn't responded after the interval, terminate its connection.
                //
                if (client.pingCount >= pingMaxRetries) {
                    Debug_1.debugConnection(`terminating unresponsive client ${client.sessionId}`);
                    return client.terminate();
                }
                client.pingCount++;
                client.ping(noop);
            });
        }, pingInterval);
    }
    onConnection(client, req) {
        return __awaiter(this, void 0, void 0, function* () {
            // prevent server crashes if a single client had unexpected error
            client.on('error', (err) => Debug_1.debugAndPrintError(err.message + '\n' + err.stack));
            client.on('pong', heartbeat);
            // compatibility with ws / uws
            const upgradeReq = req || client.upgradeReq;
            const parsedURL = url_1.default.parse(upgradeReq.url);
            const sessionId = querystring_1.default.parse(parsedURL.query).sessionId;
            const processAndRoomId = parsedURL.pathname.match(/\/[a-zA-Z0-9_\-]+\/([a-zA-Z0-9_\-]+)$/);
            const roomId = processAndRoomId && processAndRoomId[1];
            const room = matchMaker.getRoomById(roomId);
            // set client id
            client.pingCount = 0;
            // set client options
            client.id = sessionId;
            client.sessionId = sessionId;
            try {
                if (!room || !room.hasReservedSeat(sessionId)) {
                    throw new Error('seat reservation expired.');
                }
                yield room._onJoin(client, upgradeReq);
            }
            catch (e) {
                Debug_1.debugAndPrintError(e);
                Protocol_1.send[__1.Protocol.JOIN_ERROR](client, (e && e.message) || '');
                client.close(__1.Protocol.WS_CLOSE_WITH_ERROR);
            }
        });
    }
}
exports.WebSocketTransport = WebSocketTransport;
