"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const debug_1 = __importDefault(require("debug"));
const MatchMaker_1 = require("./MatchMaker");
exports.debugMatchMaking = debug_1.default('colyseus:matchmaking');
exports.debugPatch = debug_1.default('colyseus:patch');
exports.debugError = debug_1.default('colyseus:errors');
exports.debugConnection = debug_1.default('colyseus:connection');
exports.debugAndPrintError = (e) => {
    const message = (e instanceof Error) ? e.stack : e;
    if (!(e instanceof MatchMaker_1.MatchMakeError)) {
        console.error(message);
    }
    exports.debugError.call(exports.debugError, message);
};
