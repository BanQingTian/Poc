/// <reference types="node" />
import http from 'http';
import WebSocket from 'ws';
export declare enum Protocol {
    JOIN_ROOM = 10,
    JOIN_ERROR = 11,
    LEAVE_ROOM = 12,
    ROOM_DATA = 13,
    ROOM_STATE = 14,
    ROOM_STATE_PATCH = 15,
    ROOM_DATA_SCHEMA = 16,
    WS_CLOSE_NORMAL = 1000,
    WS_CLOSE_CONSENTED = 4000,
    WS_CLOSE_WITH_ERROR = 4002,
    WS_SERVER_DISCONNECT = 4201,
    WS_TOO_MANY_CLIENTS = 4202,
    ERR_MATCHMAKE_NO_HANDLER = 4210,
    ERR_MATCHMAKE_INVALID_CRITERIA = 4211,
    ERR_MATCHMAKE_INVALID_ROOM_ID = 4212,
    ERR_MATCHMAKE_UNHANDLED = 4213,
    ERR_MATCHMAKE_EXPIRED = 4214
}
export declare enum IpcProtocol {
    SUCCESS = 0,
    ERROR = 1,
    TIMEOUT = 2
}
export declare enum ClientState {
    JOINING = 0,
    JOINED = 1,
    RECONNECTED = 2
}
export declare type Client = WebSocket & {
    upgradeReq?: http.IncomingMessage;
    id: string;
    sessionId: string;
    /**
     * auth data provided by your `onAuth`
     */
    auth?: any;
    pingCount: number;
    state: ClientState;
    _enqueuedMessages: any[];
};
export declare function decode(message: any): any;
export declare const send: {
    raw: (client: Client, bytes: number[] | Buffer) => void;
    11: (client: Client, message: string) => void;
    10: (client: Client, serializerId: string, handshake?: number[]) => Promise<void>;
    14: (client: Client, bytes: number[]) => void;
    /**
     * TODO: refactor me. Move this to `SchemaSerializer` / `FossilDeltaSerializer`
     */
    13: (client: Client, message: any, encode?: boolean) => void;
};
export declare function utf8Write(buff: Buffer, offset: number, str?: string): void;
export declare function utf8Length(str?: string): number;
