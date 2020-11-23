"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
const schema_1 = require("@colyseus/schema");
const Room_1 = require("../Room");
class RoomData extends schema_1.Schema {
}
__decorate([
    schema_1.type('string')
], RoomData.prototype, "id", void 0);
__decorate([
    schema_1.type('string')
], RoomData.prototype, "name", void 0);
__decorate([
    schema_1.type('number')
], RoomData.prototype, "clients", void 0);
__decorate([
    schema_1.type('number')
], RoomData.prototype, "maxClients", void 0);
__decorate([
    schema_1.type('string')
], RoomData.prototype, "metadata", void 0);
class LobbyState extends schema_1.Schema {
}
__decorate([
    schema_1.type([RoomData])
], LobbyState.prototype, "rooms", void 0);
class LobbyRoom extends Room_1.Room {
    onCreate(options) {
        this.setState(new LobbyState());
        this.clock.setInterval(() => this.fetch(), Math.max(1, options.updateInterval || 5000) * 1000);
    }
    fetch() {
        // TODO: make .driver available on this scope!
    }
    onMessage() {
        // TODO:
    }
    onDispose() {
        // TODO:
    }
}
exports.LobbyRoom = LobbyRoom;
