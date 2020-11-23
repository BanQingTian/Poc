import { mongodClient } from "./mongoClient"
var StringBuffer = require("stringbuffer");

process.env.CreatRoomID = "CreateRoom";
process.env.JoinRoomID = "JoinRoom";
process.env.LeaveRoomID = "LeaveRoom";
process.env.StartGameID = "StartGame";
process.env.EndGameID = "EndGame";
process.env.UploadScoreID = "UploadScore";
process.env.dataCollectionID = "dataPoints";
process.env.SwitchRoomOwnerID = "SwitchRoomOwner";

export class buryingPointTool {
    _data = new Array();
    mongoClient: mongodClient;
    _id: string;
    moment = require('moment');
    _gameMatchData = new Array();

    constructor(id: string) {
        this._id = id;
        this.mongoClient = new mongodClient();
    }

    log(action: string, param: Map<string, any>) {
        param.set("action", action);
        param.set("timestamp", Date.now().valueOf());
        if (action == process.env.StartGameID
            || action == process.env.EndGameID
            || action == process.env.UploadScoreID) {
            this.oprateAction(action, param);
        }
        else {
            this._data.push(param);
        }
    }

    currentAction: string = null;
    oprateAction(action: string, param: any) {
        if (this.currentAction == null) {
            this._gameMatchData.push(param);
            this._data.push(this._gameMatchData);
            this.currentAction = action;
        }
        else if (this.currentAction != action) {
            this._gameMatchData.push(param);
            if (action == process.env.EndGameID) {
                this._gameMatchData = new Array();
                this.currentAction = null;
            }
        } else {
            this._gameMatchData = new Array();
            this._gameMatchData.push(param);
            this._data.push(this._gameMatchData);
        }
    }

    save() {
        var data = new Map<string, any>();
        data.set("_id", this._id);
        data.set("content", this._data);
        data.set("date", this.moment().format("YYYY/MM/DD HH:mm"));
        this.mongoClient.insertOrUpdate(process.env.dataCollectionID, { _id: this._id }, data);
    }
}