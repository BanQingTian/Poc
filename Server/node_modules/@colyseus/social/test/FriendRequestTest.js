"use strict";
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
var mongoose_1 = __importDefault(require("mongoose"));
var assert_1 = __importDefault(require("assert"));
var src_1 = require("../src");
var User_1 = __importDefault(require("../src/models/User"));
var FriendRequest_1 = __importDefault(require("../src/models/FriendRequest"));
var utils_1 = require("./utils");
describe("FriendRequest", function () {
    before(function () { return __awaiter(_this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: 
                // connect & clear database.
                return [4 /*yield*/, src_1.connectDatabase()];
                case 1:
                    // connect & clear database.
                    _a.sent();
                    return [4 /*yield*/, utils_1.clearTestUsers()];
                case 2:
                    _a.sent();
                    return [4 /*yield*/, utils_1.clearFriendRequests()];
                case 3:
                    _a.sent();
                    // create dummy users
                    return [4 /*yield*/, User_1.default.create([
                            { username: "jake", displayName: "Jake" },
                            { username: "snake", displayName: "Snake" },
                            { username: "katarina", displayName: "Katarina" },
                        ])];
                case 4:
                    // create dummy users
                    _a.sent();
                    return [2 /*return*/];
            }
        });
    }); });
    after(function () { return mongoose_1.default.connection.close(); });
    it("should send friend request", function () { return __awaiter(_this, void 0, void 0, function () {
        var jake, snake, friendRequest;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, User_1.default.findOne({ username: "jake" })];
                case 1:
                    jake = _a.sent();
                    return [4 /*yield*/, User_1.default.findOne({ username: "snake" })];
                case 2:
                    snake = _a.sent();
                    return [4 /*yield*/, src_1.sendFriendRequest(jake._id, snake._id)];
                case 3:
                    _a.sent();
                    return [4 /*yield*/, FriendRequest_1.default.findOne({ sender: jake._id })];
                case 4:
                    friendRequest = _a.sent();
                    assert_1.default.deepEqual(friendRequest.sender, jake._id);
                    assert_1.default.deepEqual(friendRequest.receiver, snake._id);
                    return [2 /*return*/];
            }
        });
    }); });
    it("should list friend requests", function () { return __awaiter(_this, void 0, void 0, function () {
        var snake, jake, requests;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, User_1.default.findOne({ username: "snake" })];
                case 1:
                    snake = _a.sent();
                    return [4 /*yield*/, User_1.default.findOne({ username: "jake" })];
                case 2:
                    jake = _a.sent();
                    return [4 /*yield*/, src_1.getFriendRequests(snake._id)];
                case 3:
                    requests = _a.sent();
                    assert_1.default.equal(requests.length, 1);
                    assert_1.default.deepEqual(requests[0].sender, jake._id);
                    assert_1.default.deepEqual(requests[0].receiver, snake._id);
                    return [2 /*return*/];
            }
        });
    }); });
    it("should accept friend request", function () { return __awaiter(_this, void 0, void 0, function () {
        var friendRequest, removedFriendRequest, sender, receiver;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, FriendRequest_1.default.findOne({})];
                case 1:
                    friendRequest = _a.sent();
                    return [4 /*yield*/, src_1.consumeFriendRequest(friendRequest.receiver, friendRequest.sender)];
                case 2:
                    _a.sent();
                    return [4 /*yield*/, FriendRequest_1.default.findOne({ _id: friendRequest._id })];
                case 3:
                    removedFriendRequest = _a.sent();
                    assert_1.default.equal(removedFriendRequest, null);
                    return [4 /*yield*/, User_1.default.findOne({ _id: friendRequest.sender })];
                case 4:
                    sender = _a.sent();
                    return [4 /*yield*/, User_1.default.findOne({ _id: friendRequest.receiver })];
                case 5:
                    receiver = _a.sent();
                    assert_1.default.ok(utils_1.includes(sender.friendIds, receiver._id));
                    assert_1.default.ok(utils_1.includes(receiver.friendIds, sender._id));
                    return [2 /*return*/];
            }
        });
    }); });
    it("should decline friend request", function () { return __awaiter(_this, void 0, void 0, function () {
        var jake, katarina, friendRequest, removedFriendRequest, sender, receiver;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, User_1.default.findOne({ username: "jake" })];
                case 1:
                    jake = _a.sent();
                    return [4 /*yield*/, User_1.default.findOne({ username: "katarina" })];
                case 2:
                    katarina = _a.sent();
                    return [4 /*yield*/, src_1.sendFriendRequest(jake._id, katarina._id)];
                case 3:
                    _a.sent();
                    return [4 /*yield*/, FriendRequest_1.default.findOne({ sender: jake._id })];
                case 4:
                    friendRequest = _a.sent();
                    return [4 /*yield*/, src_1.consumeFriendRequest(friendRequest.receiver, friendRequest.sender, false)];
                case 5:
                    _a.sent();
                    return [4 /*yield*/, FriendRequest_1.default.findOne({ _id: friendRequest._id })];
                case 6:
                    removedFriendRequest = _a.sent();
                    assert_1.default.equal(removedFriendRequest, null);
                    return [4 /*yield*/, User_1.default.findOne({ _id: friendRequest.sender })];
                case 7:
                    sender = _a.sent();
                    return [4 /*yield*/, User_1.default.findOne({ _id: friendRequest.receiver })];
                case 8:
                    receiver = _a.sent();
                    assert_1.default.ok(!utils_1.includes(sender.friendIds, receiver._id));
                    assert_1.default.ok(!utils_1.includes(receiver.friendIds, sender._id));
                    return [2 /*return*/];
            }
        });
    }); });
    it("shouldn't create multiple friend requests for the same user", function () { return __awaiter(_this, void 0, void 0, function () {
        var jake, katarina, i, friendRequestCount;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, User_1.default.findOne({ username: "jake" })];
                case 1:
                    jake = _a.sent();
                    return [4 /*yield*/, User_1.default.findOne({ username: "katarina" })];
                case 2:
                    katarina = _a.sent();
                    i = 0;
                    _a.label = 3;
                case 3:
                    if (!(i < 10)) return [3 /*break*/, 6];
                    return [4 /*yield*/, src_1.sendFriendRequest(jake._id, katarina._id)];
                case 4:
                    _a.sent();
                    _a.label = 5;
                case 5:
                    i++;
                    return [3 /*break*/, 3];
                case 6: return [4 /*yield*/, FriendRequest_1.default.countDocuments({})];
                case 7:
                    friendRequestCount = _a.sent();
                    assert_1.default.equal(friendRequestCount, 1);
                    return [2 /*return*/];
            }
        });
    }); });
    it("should allow to block a user", function () { return __awaiter(_this, void 0, void 0, function () {
        var jake, katarina, friendRequest;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, User_1.default.findOne({ username: "jake" })];
                case 1:
                    jake = _a.sent();
                    return [4 /*yield*/, User_1.default.findOne({ username: "katarina" })];
                case 2:
                    katarina = _a.sent();
                    return [4 /*yield*/, src_1.sendFriendRequest(jake._id, katarina._id)];
                case 3:
                    _a.sent();
                    // katarina blocks jack
                    return [4 /*yield*/, src_1.blockUser(katarina._id, jake._id)];
                case 4:
                    // katarina blocks jack
                    _a.sent();
                    return [4 /*yield*/, FriendRequest_1.default.findOne({ sender: jake._id, receiver: katarina._id })];
                case 5:
                    friendRequest = _a.sent();
                    assert_1.default.equal(friendRequest, null);
                    return [4 /*yield*/, User_1.default.findOne({ username: "jake" })];
                case 6:
                    // assert lack of presence in `friendIds` array.
                    jake = _a.sent();
                    return [4 /*yield*/, User_1.default.findOne({ username: "katarina" })];
                case 7:
                    katarina = _a.sent();
                    assert_1.default.ok(!utils_1.includes(jake.friendIds, katarina._id));
                    assert_1.default.ok(!utils_1.includes(katarina.friendIds, jake._id));
                    // assert presence in `blockedUserIds` array.
                    assert_1.default.ok(!utils_1.includes(jake.blockedUserIds, katarina._id));
                    assert_1.default.ok(utils_1.includes(katarina.blockedUserIds, jake._id));
                    // further friend requests should be ignored.
                    return [4 /*yield*/, src_1.sendFriendRequest(jake._id, katarina._id)];
                case 8:
                    // further friend requests should be ignored.
                    _a.sent();
                    return [4 /*yield*/, FriendRequest_1.default.findOne({ sender: jake._id, receiver: katarina._id })];
                case 9:
                    friendRequest = _a.sent();
                    assert_1.default.equal(friendRequest, null);
                    return [2 /*return*/];
            }
        });
    }); });
});
