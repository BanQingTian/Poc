/// <reference types="node" />
import http from 'http';
import WebSocket from 'ws';
import { Client } from '..';
import { ServerOptions } from './../Server';
import { Transport } from './Transport';
export declare class WebSocketTransport extends Transport {
    protected wss: WebSocket.Server;
    protected pingInterval: NodeJS.Timer;
    protected pingIntervalMS: number;
    protected pingMaxRetries: number;
    constructor(options: ServerOptions, engine: any);
    listen(port: number, hostname?: string, backlog?: number, listeningListener?: Function): this;
    shutdown(): void;
    protected autoTerminateUnresponsiveClients(pingInterval: number, pingMaxRetries: number): void;
    protected onConnection(client: Client, req?: http.IncomingMessage & any): Promise<void>;
}
