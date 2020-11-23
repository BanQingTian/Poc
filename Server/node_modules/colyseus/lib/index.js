"use strict";
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result["default"] = mod;
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
const timer_1 = __importStar(require("@gamestdio/timer"));
exports.Clock = timer_1.default;
exports.Delayed = timer_1.Delayed;
// Core classes
var Server_1 = require("./Server");
exports.Server = Server_1.Server;
var Room_1 = require("./Room");
exports.Room = Room_1.Room;
var Protocol_1 = require("./Protocol");
exports.Protocol = Protocol_1.Protocol;
var RegisteredHandler_1 = require("./matchmaker/RegisteredHandler");
exports.RegisteredHandler = RegisteredHandler_1.RegisteredHandler;
// MatchMaker
const matchMaker = __importStar(require("./MatchMaker"));
exports.matchMaker = matchMaker;
var LocalPresence_1 = require("./presence/LocalPresence");
exports.LocalPresence = LocalPresence_1.LocalPresence;
var RedisPresence_1 = require("./presence/RedisPresence");
exports.RedisPresence = RedisPresence_1.RedisPresence;
// Default rooms
var RelayRoom_1 = require("./rooms/RelayRoom");
exports.RelayRoom = RelayRoom_1.RelayRoom;
// Serializers
var FossilDeltaSerializer_1 = require("./serializer/FossilDeltaSerializer");
exports.FossilDeltaSerializer = FossilDeltaSerializer_1.FossilDeltaSerializer;
var SchemaSerializer_1 = require("./serializer/SchemaSerializer");
exports.SchemaSerializer = SchemaSerializer_1.SchemaSerializer;
var nonenumerable_1 = require("nonenumerable");
exports.nosync = nonenumerable_1.nonenumerable;
var Utils_1 = require("./Utils");
exports.generateId = Utils_1.generateId;
exports.Deferred = Utils_1.Deferred;
