import { Schema, type, MapSchema, ArraySchema } from "@colyseus/schema";
import { number } from "@colyseus/schema/lib/encoding/decode";

export enum MsgId {
    // Connect server
    Connect = 0,

    // Disconnect from server
    DisConnect = 1,

    // Start game
    StartGame = 2,

    // End game
    EndGame = 3,

    //Create a net object
    CreateANetObject = 4,

    //Destroy a net object
    DestroyANetObject = 5,

    // Command
    Commond = 6,

    // Update scene data
    UpdateRoomInfo = 7,

    // Heart beat package
    HeartBeat = 8,

    UploadScores = 9,

    UploadDeviceInfo = 10,
}

export enum Target {
    Owner = 0,
    All = 1,
    Server = 2,
    Other = 3,
}

export class ConnectResult {
    result: boolean;
    version: string;
}

//typeof Schema
export class NetVector3 extends Schema {
    @type("float32") x: number = 0;
    @type("float32") y: number = 0;
    @type("float32") z: number = 0;
}

//typeof Schema
export class NetQuaternion extends Schema {
    @type("float32") x: number = 0;
    @type("float32") y: number = 0;
    @type("float32") z: number = 0;
    @type("float32") w: number = 0;
}

export class Entity extends Schema {
    @type("string") id: string;
    @type("string") owner: string;
    @type("int32") type: number = 0;
    @type("string") name: string;
    @type(NetVector3) position: NetVector3;
    @type(NetQuaternion) rotation: NetQuaternion;
    @type("string") extraInfo: string;
}

export class Player extends Entity {
    @type("boolean") isconnect: Boolean = true;
}

export class Enemy extends Entity {
    @type("float32") healthy: number = 0;
}

export class CommondInfo extends Schema {
    @type("string") func: string;        // function name
    @type("string") param: string;       // function param (split by ',')
}

export class RoomState extends Schema {
    @type({ map: Entity })
    entities = new MapSchema<Entity>();

    @type("string") roomID: string;       // room's id
    @type("string") roomName: string;       // room's name
    @type("string") psw: string;           // room's psw 
    @type("string") owner: string;       // room's owner
    @type("int32") state: number;       // room's state   0 =  unstart,  1= started
}

export class Header extends Schema {
    @type("int32") target: number;
    @type("int32") msgID: number;
}

// Demonstrate sending schema data types as messages
export class Message extends Schema {
    @type(Header) header: Header;
    @type("string") content: string;
}

export enum EntityType {
    Player = 0,
    Normal = 1,
    Enemy = 2,
}

export function isMessage(obj: Message | Entity): obj is Message {
    return (obj as Message).header !== undefined;
}

export function isEntity(obj: Message | Entity): obj is Entity {
    return (obj as Entity).extraInfo !== undefined;
}

export function isEnemy(obj: Enemy | Entity): obj is Enemy {
    return (obj as Enemy).healthy !== undefined;
}

export function copy(source: Entity, destiny: Entity) {
    if (source == null || destiny == null) {
        return;
    }
    destiny.position.x = source.position.x;
    destiny.position.y = source.position.y;
    destiny.position.z = source.position.z;

    destiny.rotation.x = source.rotation.x;
    destiny.rotation.y = source.rotation.y;
    destiny.rotation.z = source.rotation.z;
    destiny.rotation.w = source.rotation.w;

    destiny.extraInfo = source.extraInfo;

    if (isEnemy(destiny)) {
        destiny.healthy = (source as Enemy).healthy;
    }
}

export function createAEntity(sessionid: string) {
    var entity = new Entity();
    entity.id = guid();
    entity.type = EntityType.Normal;
    entity.owner = sessionid;
    var pos = new NetVector3();
    pos.x = 0;
    pos.y = 0;
    pos.z = 0;
    entity.position = pos;
    var quaternion = new NetQuaternion();
    quaternion.x = 0;
    quaternion.y = 0;
    quaternion.z = 0;
    quaternion.w = 0;
    entity.rotation = quaternion;
    return entity;
}

export function createAPlayer(sessionid: string) {
    var player = new Player();
    player.id = guid();
    player.type = EntityType.Player;
    player.owner = sessionid;
    var pos = new NetVector3();
    pos.x = 0;
    pos.y = 0;
    pos.z = 0;
    player.position = pos;
    var quaternion = new NetQuaternion();
    quaternion.x = 0;
    quaternion.y = 0;
    quaternion.z = 0;
    quaternion.w = 0;
    player.rotation = quaternion;
    player.isconnect = true;
    return player;
}

export function createAEnemy(sessionid: string) {
    var enemy = new Enemy();
    enemy.id = guid();
    enemy.type = EntityType.Enemy;
    enemy.owner = sessionid;
    var pos = new NetVector3();
    pos.x = 0;
    pos.y = 0;
    pos.z = 0;
    enemy.position = pos;
    var quaternion = new NetQuaternion();
    quaternion.x = 0;
    quaternion.y = 0;
    quaternion.z = 0;
    quaternion.w = 0;
    enemy.rotation = quaternion;
    enemy.healthy = 100;
    return enemy;
}

export function getEntityByType(type: number, sessionid: string) {
    if (type == EntityType.Player) {
        return createAPlayer(sessionid);
    }
    else if (type == EntityType.Normal) {
        return createAEntity(sessionid);
    }
    else if (type == EntityType.Enemy) {
        // enemy dose'nt have a owner
        return createAEnemy(sessionid);
    }
}

function guid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

export function len(map: MapSchema<Entity>) {
    var i = 0;
    for (let item in map) {
        i++;
    }
    return i;
}