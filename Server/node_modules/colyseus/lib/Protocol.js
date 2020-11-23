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
const ws_1 = __importDefault(require("ws"));
const Debug_1 = require("./Debug");
// Colyseus protocol codes range between 0~100
var Protocol;
(function (Protocol) {
    // Room-related (10~19)
    Protocol[Protocol["JOIN_ROOM"] = 10] = "JOIN_ROOM";
    Protocol[Protocol["JOIN_ERROR"] = 11] = "JOIN_ERROR";
    Protocol[Protocol["LEAVE_ROOM"] = 12] = "LEAVE_ROOM";
    Protocol[Protocol["ROOM_DATA"] = 13] = "ROOM_DATA";
    Protocol[Protocol["ROOM_STATE"] = 14] = "ROOM_STATE";
    Protocol[Protocol["ROOM_STATE_PATCH"] = 15] = "ROOM_STATE_PATCH";
    Protocol[Protocol["ROOM_DATA_SCHEMA"] = 16] = "ROOM_DATA_SCHEMA";
    // WebSocket close codes (https://github.com/Luka967/websocket-close-codes)
    Protocol[Protocol["WS_CLOSE_NORMAL"] = 1000] = "WS_CLOSE_NORMAL";
    // WebSocket error codes
    Protocol[Protocol["WS_CLOSE_CONSENTED"] = 4000] = "WS_CLOSE_CONSENTED";
    Protocol[Protocol["WS_CLOSE_WITH_ERROR"] = 4002] = "WS_CLOSE_WITH_ERROR";
    Protocol[Protocol["WS_SERVER_DISCONNECT"] = 4201] = "WS_SERVER_DISCONNECT";
    Protocol[Protocol["WS_TOO_MANY_CLIENTS"] = 4202] = "WS_TOO_MANY_CLIENTS";
    // MatchMaking Error Codes
    Protocol[Protocol["ERR_MATCHMAKE_NO_HANDLER"] = 4210] = "ERR_MATCHMAKE_NO_HANDLER";
    Protocol[Protocol["ERR_MATCHMAKE_INVALID_CRITERIA"] = 4211] = "ERR_MATCHMAKE_INVALID_CRITERIA";
    Protocol[Protocol["ERR_MATCHMAKE_INVALID_ROOM_ID"] = 4212] = "ERR_MATCHMAKE_INVALID_ROOM_ID";
    Protocol[Protocol["ERR_MATCHMAKE_UNHANDLED"] = 4213] = "ERR_MATCHMAKE_UNHANDLED";
    Protocol[Protocol["ERR_MATCHMAKE_EXPIRED"] = 4214] = "ERR_MATCHMAKE_EXPIRED";
})(Protocol = exports.Protocol || (exports.Protocol = {}));
// Inter-process communication protocol
var IpcProtocol;
(function (IpcProtocol) {
    IpcProtocol[IpcProtocol["SUCCESS"] = 0] = "SUCCESS";
    IpcProtocol[IpcProtocol["ERROR"] = 1] = "ERROR";
    IpcProtocol[IpcProtocol["TIMEOUT"] = 2] = "TIMEOUT";
})(IpcProtocol = exports.IpcProtocol || (exports.IpcProtocol = {}));
var ClientState;
(function (ClientState) {
    ClientState[ClientState["JOINING"] = 0] = "JOINING";
    ClientState[ClientState["JOINED"] = 1] = "JOINED";
    ClientState[ClientState["RECONNECTED"] = 2] = "RECONNECTED";
})(ClientState = exports.ClientState || (exports.ClientState = {}));
function decode(message) {
    try {
        message = notepack_io_1.default.decode(Buffer.from(message));
    }
    catch (e) {
        Debug_1.debugAndPrintError(`message couldn't be decoded: ${message}\n${e.stack}`);
        return;
    }
    return message;
}
exports.decode = decode;
exports.send = {
    raw: (client, bytes) => {
        if (client.readyState !== ws_1.default.OPEN) {
            return;
        }
        if (client.state === ClientState.JOINING) {
            // sending messages during `onJoin`.
            // - the client-side cannot register "onMessage" callbacks at this point.
            // - enqueue the messages to be send after JOIN_ROOM message has been sent
            client._enqueuedMessages.push(bytes);
            return;
        }
        client.send(bytes, { binary: true });
    },
    [Protocol.JOIN_ERROR]: (client, message) => {
        if (client.readyState !== ws_1.default.OPEN) {
            return;
        }
        const buff = Buffer.allocUnsafe(1 + utf8Length(message));
        buff.writeUInt8(Protocol.JOIN_ERROR, 0);
        utf8Write(buff, 1, message);
        client.send(buff, { binary: true });
    },
    [Protocol.JOIN_ROOM]: (client, serializerId, handshake) => __awaiter(void 0, void 0, void 0, function* () {
        if (client.readyState !== ws_1.default.OPEN) {
            return;
        }
        let offset = 0;
        const serializerIdLength = utf8Length(serializerId);
        const handshakeLength = (handshake) ? handshake.length : 0;
        const buff = Buffer.allocUnsafe(1 + serializerIdLength + handshakeLength);
        buff.writeUInt8(Protocol.JOIN_ROOM, offset++);
        utf8Write(buff, offset, serializerId);
        offset += serializerIdLength;
        if (handshake) {
            for (let i = 0, l = handshake.length; i < l; i++) {
                buff.writeUInt8(handshake[i], offset++);
            }
        }
        client.send(buff, { binary: true });
    }),
    [Protocol.ROOM_STATE]: (client, bytes) => {
        exports.send.raw(client, [Protocol.ROOM_STATE, ...bytes]);
    },
    // [Protocol.ROOM_STATE_PATCH]: (client: Client, bytes: number[]) => {
    //   if (
    //     client.state === ClientState.JOINING &&
    //     client.readyState !== WebSocket.OPEN
    //   ) {
    //     return;
    //   }
    //   console.log({ bytes });
    //   client.send(Buffer.alloc(1, Protocol.ROOM_STATE_PATCH), { binary: true });
    //   client.send(bytes, { binary: true });
    // },
    /**
     * TODO: refactor me. Move this to `SchemaSerializer` / `FossilDeltaSerializer`
     */
    [Protocol.ROOM_DATA]: (client, message, encode = true) => {
        exports.send.raw(client, [Protocol.ROOM_DATA, ...(encode && notepack_io_1.default.encode(message) || message)]);
    },
};
function utf8Write(buff, offset, str = '') {
    buff[offset++] = utf8Length(str) - 1;
    let c = 0;
    for (let i = 0, l = str.length; i < l; i++) {
        c = str.charCodeAt(i);
        if (c < 0x80) {
            buff[offset++] = c;
        }
        else if (c < 0x800) {
            buff[offset++] = 0xc0 | (c >> 6);
            buff[offset++] = 0x80 | (c & 0x3f);
        }
        else if (c < 0xd800 || c >= 0xe000) {
            buff[offset++] = 0xe0 | (c >> 12);
            buff[offset++] = 0x80 | (c >> 6) & 0x3f;
            buff[offset++] = 0x80 | (c & 0x3f);
        }
        else {
            i++;
            c = 0x10000 + (((c & 0x3ff) << 10) | (str.charCodeAt(i) & 0x3ff));
            buff[offset++] = 0xf0 | (c >> 18);
            buff[offset++] = 0x80 | (c >> 12) & 0x3f;
            buff[offset++] = 0x80 | (c >> 6) & 0x3f;
            buff[offset++] = 0x80 | (c & 0x3f);
        }
    }
}
exports.utf8Write = utf8Write;
// Faster for short strings than Buffer.byteLength
function utf8Length(str = '') {
    let c = 0;
    let length = 0;
    for (let i = 0, l = str.length; i < l; i++) {
        c = str.charCodeAt(i);
        if (c < 0x80) {
            length += 1;
        }
        else if (c < 0x800) {
            length += 2;
        }
        else if (c < 0xd800 || c >= 0xe000) {
            length += 3;
        }
        else {
            i++;
            length += 4;
        }
    }
    return length + 1;
}
exports.utf8Length = utf8Length;
