"use strict";
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
var _this = this;
Object.defineProperty(exports, "__esModule", { value: true });
var assert_1 = __importDefault(require("assert"));
var express_1 = __importDefault(require("express"));
var mongoose_1 = __importDefault(require("mongoose"));
var httpie_1 = require("httpie");
var express_2 = __importDefault(require("../../express"));
var utils_1 = require("../utils");
var facebook_1 = require("../../src/facebook");
var src_1 = require("../../src");
describe("Express", function () {
    var TESTPORT = 2267;
    var app = express_1.default();
    app.use("/", express_2.default);
    // spin-up and tear-down express server for testing
    var server;
    before(function (done) {
        src_1.connectDatabase(function (err) {
            server = app.listen(TESTPORT, done);
        });
    });
    after(function () {
        server.close();
        mongoose_1.default.connection.close();
    });
    // 'get' utility
    var get = function (url, headers) { return __awaiter(_this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, httpie_1.get("http://localhost:" + TESTPORT + url, {
                        headers: __assign({}, headers, { 'Accept': 'application/json' })
                    })];
                case 1: return [2 /*return*/, _a.sent()];
            }
        });
    }); };
    // 'post' utility
    var post = function (url, headers) { return __awaiter(_this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, httpie_1.post("http://localhost:" + TESTPORT + url, {
                        headers: __assign({}, headers, { 'Accept': 'application/json' })
                    })];
                case 1: return [2 /*return*/, _a.sent()];
            }
        });
    }); };
    // 'put' utility
    var put = function (url, headers) { return __awaiter(_this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, httpie_1.put("http://localhost:" + TESTPORT + url, {
                        headers: __assign({}, headers, { 'Accept': 'application/json' })
                    })];
                case 1: return [2 /*return*/, _a.sent()];
            }
        });
    }); };
    // 'del' utility
    var del = function (url, headers) { return __awaiter(_this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, httpie_1.del("http://localhost:" + TESTPORT + url, {
                        headers: __assign({}, headers, { 'Accept': 'application/json' })
                    })];
                case 1: return [2 /*return*/, _a.sent()];
            }
        });
    }); };
    var loginRequest = function (fbAccessToken) { return __awaiter(_this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, post("/auth?accessToken=" + fbAccessToken)];
                case 1: return [2 /*return*/, _a.sent()];
            }
        });
    }); };
    it("shouldn't sign in with invalid access token", function () { return __awaiter(_this, void 0, void 0, function () {
        var e_1;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    _a.trys.push([0, 2, , 3]);
                    return [4 /*yield*/, post("/auth?accessToken=invalid%20token")];
                case 1:
                    _a.sent();
                    return [3 /*break*/, 3];
                case 2:
                    e_1 = _a.sent();
                    assert_1.default.equal(e_1.statusCode, 401);
                    assert_1.default.equal(e_1.data.error, "Invalid OAuth access token.");
                    return [3 /*break*/, 3];
                case 3: return [2 /*return*/];
            }
        });
    }); });
    it("should register with valid access token", function () { return __awaiter(_this, void 0, void 0, function () {
        var accessToken, facebookData, response;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, utils_1.getTestUsersAccessTokens()];
                case 1:
                    accessToken = (_a.sent())[0];
                    return [4 /*yield*/, facebook_1.getFacebookUser(accessToken)];
                case 2:
                    facebookData = _a.sent();
                    return [4 /*yield*/, loginRequest(accessToken)];
                case 3:
                    response = _a.sent();
                    assert_1.default.equal(response.statusCode, 200);
                    assert_1.default.equal(response.data.facebookId, facebookData.id);
                    return [2 /*return*/];
            }
        });
    }); });
    it("should get a list of online friends", function () { return __awaiter(_this, void 0, void 0, function () {
        var accessToken, jwt, friendsResponse, friends, friendNames;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, utils_1.getTestUsersAccessTokens()];
                case 1:
                    accessToken = (_a.sent())[1];
                    return [4 /*yield*/, loginRequest(accessToken)];
                case 2:
                    jwt = (_a.sent()).data.token;
                    return [4 /*yield*/, get("/friends/online", { authorization: "Bearer " + jwt })];
                case 3:
                    friendsResponse = _a.sent();
                    assert_1.default.equal(friendsResponse.statusCode, 200);
                    friends = friendsResponse.data;
                    friendNames = friends.map(function (friend) { return friend.displayName; });
                    assert_1.default.deepEqual(Object.keys(friends[0]), ['username', 'displayName', 'avatarUrl', '_id']);
                    assert_1.default.ok(friends.length > 0);
                    assert_1.default.ok(friendNames.indexOf("Rick") >= 0);
                    assert_1.default.ok(friendNames.indexOf("Open") >= 0);
                    assert_1.default.ok(friendNames.indexOf("Bob") >= 0);
                    assert_1.default.ok(friendNames.indexOf("Maria") >= 0);
                    return [2 /*return*/];
            }
        });
    }); });
});
