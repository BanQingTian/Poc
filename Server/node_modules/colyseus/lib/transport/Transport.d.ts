/// <reference types="node" />
import * as http from 'http';
import * as https from 'https';
import * as net from 'net';
export declare abstract class Transport {
    server: net.Server | http.Server | https.Server;
    abstract listen(port?: number, hostname?: string, backlog?: number, listeningListener?: Function): this;
    abstract shutdown(): void;
    address(): net.AddressInfo;
}
export { TCPTransport } from './TCPTransport';
export { WebSocketTransport } from './WebSocketTransport';
