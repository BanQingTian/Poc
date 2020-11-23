import { Schema } from '@colyseus/schema';
import { Room } from '../Room';
declare class RoomData extends Schema {
    id: string;
    name: string;
    clients: number;
    maxClients: number;
    metadata: string;
}
declare class LobbyState extends Schema {
    rooms: RoomData[];
}
export declare class LobbyRoom extends Room<LobbyState> {
    onCreate(options: any): void;
    fetch(): void;
    onMessage(): void;
    onDispose(): void;
}
export {};
