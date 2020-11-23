import { Room, Client, generateId } from "colyseus";

import {
  RoomState, Message, Header, NetVector3,
  NetQuaternion, MsgId, Target, Entity, EntityType,
  isMessage, isEntity, copy, getEntityByType, len, Enemy
} from "./Entity"

import { roomCache } from "./roomCache"
import { getLoger } from "./logs/logger"
import { buryingPointTool } from "./buryingDataPoint"
import { mongodClient } from "./mongoClient"
import { json } from "body-parser";

const PSW_MISMATCH_ERROCODE: Number = 1000;
const JOIN_ROOM_NOTALLOWED_ERROCODE: Number = 1001;
const ALREADY_INROOM_ERROCODE: Number = 1002;
var clientDict = new Map<string, string>();   // cache the uuid of client
var hearBeatMap = new Map<string, number>();   // cache the heart beat of client
var players = new Map<string, Entity>();     // cahce the players of a room
var ipDict = new Map<string, string>();      // cache the ip address of client
var deviceinfoDict = new Map<string, any>(); // cache the device info of a client
var logger = getLoger("NrealRoom");

export class NrealRoom extends Room {
  pointTool: buryingPointTool;

  onCreate(options: any) {
    // this.roomsCache = new roomCache();
    let str: string = `NrealRoom created name:${options["roomName"]} psw:${options["psw"]} uuid:${options["uuid"]}`;
    logger.info(str);
    var room = new RoomState();
    room.roomName = options["roomName"];
    room.psw = options["psw"];
    room.roomID = this.roomId;
    this.pointTool = new buryingPointTool(this.roomId + this.getTimeStamp());

    this.setState(room);
    this.setPatchRate(1000 / 20);
    this.setSimulationInterval((dt) => this.update(dt));
  }

  async onAuth(client, options) {
    if (this.state.psw != options["psw"]) {
      logger.info("Pass word of the room is mismatch!");
      throw new Error(`${PSW_MISMATCH_ERROCODE}:The psw is mismatch!`);
    }
    if (this.state.state == 1) {
      logger.info("The game has started. You are not allowed to join the room!");
      throw new Error(`${JOIN_ROOM_NOTALLOWED_ERROCODE}:The game has started. You are not allowed to join the room!`);
    }
    if (clientDict.has(options["uuid"])) {
      logger.info("The client already in a room. You are not allowed to join the room!");
      throw new Error(`${ALREADY_INROOM_ERROCODE}:The client already in a room.`);
    }
    logger.info("on auth success!");
    return this.state;
  }

  async onJoin(client: Client, options: any) {
    clientDict.set(options["uuid"], client.sessionId);

    ipDict.set(client.sessionId, options.ip);
    var pointdata = new Map<string, any>([
      ["clientIp", options.ip],
      ["sn", deviceinfoDict.get(client.sessionId)],
      ["sessionId", client.sessionId],
      ["roomName", this.state.roomName]
    ]);
    if (this.state.owner == null) {
      this.pointTool.log(process.env.CreatRoomID, pointdata);
    } else {
      this.pointTool.log(process.env.JoinRoomID, pointdata);
    }
    let str: string = `client ${client.sessionId} joined the room:${this.state.roomName} owner:${this.state.owner}`;
    //client ip: client.upgradeReq.socket.remoteAddress
    logger.info(str);
    if (len(this.state.entities) == 0) {
      this.state.owner = client.sessionId;
      this.state.state = 0;
      this.updateRoomMetaData();
    }
    // logger.info(`Cache the room:${this.state.roomName} owner:${this.state.owner}`);
    this.addAEntity(EntityType.Player, client.sessionId);
    this.updateTimeStamp(client.sessionId);
    logger.info(`current client num: ${this.clients.length}`);
  }

  mongohelper: mongodClient;
  async updateRoomMetaData() {
    logger.info(`updateRoomMetaData name:${this.state.roomName} owner:${this.state.owner} state:${this.state.state}`);
    this.setMetadata({ roomInfo: JSON.stringify(this.state) });
    if (this.mongohelper == null) {
      this.mongohelper = new mongodClient();
    }
    var data = new Map<string, any>();
    data.set('metadata', { roomInfo: JSON.stringify(this.state) });
    this.mongohelper.insertOrUpdate('roomcaches', { roomId: this.state.roomID }, data);
  }

  addAEntity(type: number, sessionid: string) {
    logger.info(`Create  a entity: ${type}  id:${sessionid}`);
    var entity = getEntityByType(type, sessionid);
    this.state.entities[entity.id] = entity;
    if (entity.type == EntityType.Player) {
      players.set(entity.owner, entity);
    }
  }

  deleteEntityByOwner(owner: string) {
    logger.info(`Delete all entity of the owner: ${owner}`);
    for (let id in this.state.entities) {
      if (this.state.entities[id].owner == owner && this.state.entities[id].type == EntityType.Player) {
        delete this.state.entities[id];
      }
    }
  }

  findEntity(id: string) {
    for (let id in this.state.entities) {
      if (this.state.entities[id].id == id) {
        return this.state.entities[id];
      }
    }
    return null;
  }

  sendMsg(client: Client, msg: Message) {
    switch (msg.header.target) {
      case Target.All:
        this.broadcast(msg)
        break;
      case Target.Other:
        this.broadcast(msg, { except: client });
        break;
      case Target.Owner:
        this.send(client, msg);
        break;
      case Target.Server:
      default:
        break;
    }
  }

  async onLeave(client: Client, consented: boolean) {
    var pointdata = new Map<string, any>([
      ["clientIp", ipDict.get(client.sessionId)],
      ["sn", deviceinfoDict.get(client.sessionId)],
      ["sessionId", client.sessionId],
      ["roomName", this.state.roomName]
    ]);
    this.pointTool.log(process.env.LeaveRoomID, pointdata);

    logger.info(`${client.sessionId} Leave the room!`);

    // delete the client
    this.deleteEntityByOwner(client.sessionId);
    hearBeatMap.delete(client.sessionId);
    players.delete(client.sessionId);
    clientDict.forEach((value, key) => {
      if (value == client.sessionId) {
        clientDict.delete(key);
      }
    });

    if (players.size > 0) {
      players.forEach((value, key) => {
        this.state.owner = key;
        var pointdata = new Map<string, any>([
          ["clientIp", ipDict.get(this.state.owner)],
          ["sn", deviceinfoDict.get(this.state.owner)],
          ["sessionId", this.state.owner],
          ["roomName", this.state.roomName]
        ]);
        this.pointTool.log(process.env.SwitchRoomOwnerID, pointdata);

        logger.info(`Next room owner is: ${this.state.owner}`);
        logger.info(`Update the cache room:${this.state.roomName} owner:${this.state.owner}`);
        this.updateRoomMetaData();
        return;
      });
    }
  }

  onMessage(client: Client, data: any) {
    if (isMessage(data)) {
      this.oprateMessage(client, data as Message);
    }
    else if (isEntity(data)) {
      // update the entity
      var source = data as Entity;
      var entity = this.state.entities[source.id];
      copy(source, entity);
    }
  }

  oprateMessage(client: Client, msg: Message) {
    // logger.info("oprateMessage client:" + client.id + " msg id:" + msg.header.msgID + " content:" + msg.content);
    switch (msg.header.msgID) {
      case MsgId.StartGame:
        var pointdata = new Map<string, any>([
          ["clientIp", ipDict.get(client.sessionId)],
          ["sn", deviceinfoDict.get(client.sessionId)],
          ["sessionId", client.sessionId],
          ["roomName", this.state.roomName]
        ]);
        this.pointTool.log(process.env.StartGameID, pointdata);
        this.sendMsg(client, msg);
        this.state.state = 1;
        this.updateRoomMetaData();
        logger.info("StartGame:" + msg.content);
        break;
      case MsgId.EndGame:
        var pointdata = new Map<string, any>([
          ["clientIp", ipDict.get(client.sessionId)],
          ["sn", deviceinfoDict.get(client.sessionId)],
          ["sessionId", client.sessionId],
          ["roomName", this.state.roomName]
        ]);
        this.pointTool.log(process.env.EndGameID, pointdata);
        this.sendMsg(client, msg);
        this.state.state = 0;
        this.updateRoomMetaData();
        logger.info("StartGame:" + msg.content);
        break;
      case MsgId.UploadScores:
        var pointdata = new Map<string, any>([
          ["roomName", this.state.roomName],
          ["scores", JSON.parse(msg.content)]
        ]);
        this.pointTool.log(process.env.UploadScoreID, pointdata);
        break;
      case MsgId.HeartBeat:
        msg.header.target = Target.Owner;
        this.updateTimeStamp(client.sessionId);
        this.sendMsg(client, msg);
        break;
      case MsgId.CreateANetObject:
        // content is the type of entity
        this.addAEntity(Number(JSON.parse(msg.content).value__), client.sessionId);
        break;
      case MsgId.DestroyANetObject:
        // content is the id of entity
        delete this.state.entities[msg.content];
        break;
      case MsgId.Commond:
        var commond = JSON.parse(msg.content);
        if (commond.fun == "damage") {
          var param = commond.param.split(",");
          var entity = this.findEntity(param[0]);
          if (entity != null) {
            entity.healthy -= parseFloat(param[1]);
          }
          else {
            logger.info(`can not find the entity: ${param[0]}`);
          }
        }
        this.sendMsg(client, msg);
        break;
      case MsgId.UploadDeviceInfo:
        var deviceinfo = JSON.parse(msg.content);
        deviceinfoDict.set(client.sessionId, deviceinfo);
        break;
      default:
        // this.sendMsg(client, msg);
        logger.info(`Receive the msg: ${msg.header.msgID} content:${msg.content} from client: ${client.id}`);
        break
    }
  }

  update(dt?: number) {
    this.heartBeat();
  }

  updateTimeStamp(sessionid: string) {
    var currenttimestamp = this.getTimeStamp();
    hearBeatMap.set(sessionid, currenttimestamp);
  }

  heartBeat() {
    var currenttimestamp = this.getTimeStamp();
    hearBeatMap.forEach((value, key) => {
      if ((currenttimestamp - value) > 5000) {
        logger.info("Can not receive the heart beat package from client:" + key + " force to quit it!");
        let client: Client = this.findClient(key);
        if (client != null) {
          this.leaveRoom(client);
        }
      }
    });
  }

  leaveRoom(client: Client) {
    this.onLeave(client, false);
    if (players.size == 0) {
      // close the room
      this.disconnect();
      logger.info("The number of player is zero, close the room!");
    }
  }

  findClient(sessionid: string): Client {
    for (var i = 0; i < this.clients.length; i++) {
      if (this.clients[i].sessionId == sessionid) {
        return this.clients[i];
      }
    }
    return null;
  }

  getTimeStamp() {
    return new Date().getTime();
  }

  onDispose() {
    logger.info("Disposing NrealRoom...");
    this.pointTool.save();
  }
}
