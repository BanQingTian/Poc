import { MapSchema, Schema } from '@colyseus/schema';
import { Client } from '../Protocol';
import { Room } from '../Room';
declare class Player extends Schema {
    connected: boolean;
    sessionId: string;
}
declare class State extends Schema {
    players: MapSchema<Player>;
}
/**
 * client.joinOrCreate("relayroom", {
 *   maxClients: 10,
 *   allowReconnectionTime: 20
 * });
 */
export declare class RelayRoom extends Room<State> {
    allowReconnectionTime: number;
    onCreate(options: any): void;
    onJoin(client: Client, options: any): void;
    onMessage(client: Client, message: any): void;
    onLeave(client: Client, consented: boolean): Promise<void>;
}
export {};
