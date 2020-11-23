/// <reference types="node" />
import http from 'http';
import Clock from '@gamestdio/timer';
import { EventEmitter } from 'events';
import { Presence } from './presence/Presence';
import { Client } from './Protocol';
import { Deferred } from './Utils';
import { RoomListingData } from './matchmaker/drivers/Driver';
export declare const DEFAULT_SEAT_RESERVATION_TIME: number;
export declare type SimulationCallback = (deltaTime: number) => void;
export declare type RoomConstructor<T = any> = new (presence?: Presence) => Room<T>;
export interface BroadcastOptions {
    except?: Client;
    afterNextPatch?: boolean;
}
export declare enum RoomInternalState {
    CREATING = 0,
    CREATED = 1,
    DISCONNECTING = 2
}
export declare abstract class Room<State = any, Metadata = any> extends EventEmitter {
    get locked(): boolean;
    listing: RoomListingData<Metadata>;
    clock: Clock;
    roomId: string;
    roomName: string;
    maxClients: number;
    patchRate: number;
    autoDispose: boolean;
    state: State;
    presence: Presence;
    clients: Client[];
    /** @internal */
    _internalState: RoomInternalState;
    protected seatReservationTime: number;
    protected reservedSeats: {
        [sessionId: string]: any;
    };
    protected reservedSeatTimeouts: {
        [sessionId: string]: NodeJS.Timer;
    };
    protected reconnections: {
        [sessionId: string]: Deferred;
    };
    private _serializer;
    private _afterNextPatchBroadcasts;
    private _simulationInterval;
    private _patchInterval;
    private _locked;
    private _lockedExplicitly;
    private _maxClientsReached;
    private _autoDisposeTimeout;
    constructor(presence?: Presence);
    abstract onMessage(client: Client, data: any): void;
    onCreate?(options: any): void | Promise<any>;
    onJoin?(client: Client, options?: any, auth?: any): void | Promise<any>;
    onLeave?(client: Client, consented?: boolean): void | Promise<any>;
    onDispose?(): void | Promise<any>;
    onAuth(client: Client, options: any, request?: http.IncomingMessage): any | Promise<any>;
    hasReachedMaxClients(): boolean;
    setSeatReservationTime(seconds: number): this;
    hasReservedSeat(sessionId: string): boolean;
    setSimulationInterval(callback: SimulationCallback, delay?: number): void;
    setPatchRate(milliseconds: number): void;
    setState(newState: State): void;
    setMetadata(meta: Partial<Metadata>): void;
    setPrivate(bool?: boolean): Promise<any>;
    get metadata(): Metadata;
    lock(): Promise<any>;
    unlock(): Promise<any>;
    send(client: Client, message: any): void;
    broadcast(message: any, options?: BroadcastOptions): boolean;
    disconnect(): Promise<any>;
    ['_onJoin'](client: Client, req?: http.IncomingMessage): Promise<void>;
    protected sendState(client: Client): void;
    protected broadcastPatch(): boolean;
    protected broadcastAfterPatch(): void;
    protected allowReconnection(previousClient: Client, seconds?: number): Deferred;
    protected _reserveSeat(sessionId: string, joinOptions?: any, seconds?: number, allowReconnection?: boolean): Promise<boolean>;
    protected resetAutoDisposeTimeout(timeoutInSeconds: number): void;
    protected _disposeIfEmpty(): boolean;
    protected _dispose(): Promise<any>;
    private _onMessage;
    private _forciblyCloseClient;
    private _onLeave;
    private _incrementClientCount;
    private _decrementClientCount;
}
