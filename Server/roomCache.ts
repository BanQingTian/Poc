import { mongodClient } from "./mongoClient"
import {
    RoomState, Message, Header, NetVector3,
    NetQuaternion, MsgId, Target, Entity, EntityType,
    isMessage, isEntity, copy, getEntityByType, len, Enemy
} from "./Entity"
import { number } from "@colyseus/schema/lib/encoding/decode";
const mongoose = require('mongoose');

var StringBuffer = require("stringbuffer");
const roomCollection: string = "Rooms";
const roomSchema = new mongoose.Schema({
    roomID: String,
    roomName: String,
    owner: String,
    roomState: Number,
    timeStamp: Number
});

const Room = mongoose.model('room', roomSchema);

export class roomCache {
    moment = require('moment');

    constructor() {
        mongoose.connect(process.env.MONGO_URI, { useNewUrlParser: true, useUnifiedTopology: true })
            .then(() => console.log('Connected to MongoDB...'))
            .catch(err => console.error('Could not connect to MongoDB... ', err));
    }

    async getAllRooms() {
        const rooms = await Room
            .find({ timeStamp: this.geTimeStampOfHour() })
            .limit(10)
            .sort({ timeStamp: 1 })
        console.log(rooms);
    }

    async insertRooms(roomstate: RoomState) {
        const room = new Room({
            roomName: roomstate.roomName,
            roomID: roomstate.roomID,
            owner: roomstate.owner,
            roomState: roomstate.state,
            timeStamp: this.geTimeStampOfHour()
        });

        const result = await room.save();
        console.log("insertRooms" + result);
    }

    async deleteRoom(roomstate: RoomState) {
        const room = await Room.findOneAndDelete({ roomID: roomstate.roomID });
        console.log("deleteRoom" + room);
    }

    async updateOrInsertRoom(roomstate: RoomState) {
        const room = await Room.find({ roomID: roomstate.roomID });
        if (room == null) {
            await this.insertRooms(roomstate);
            return;
        };
        room.roomName = roomstate.roomName;
        room.roomID = roomstate.roomID;
        room.owner = roomstate.owner;
        room.roomState = roomstate.state;
        room.save().then(function (err, result) {
            console.log('User Created');
        });;
    }

    geTimeStampOfHour() {
        return new Date().getHours();
    }
}