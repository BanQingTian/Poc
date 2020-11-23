"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const Protocol_1 = require("../Protocol");
class MatchMakeError extends Error {
    constructor(message, code = Protocol_1.Protocol.ERR_MATCHMAKE_UNHANDLED) {
        super(message);
        this.code = code;
    }
}
exports.MatchMakeError = MatchMakeError;
