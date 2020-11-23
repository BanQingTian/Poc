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
Object.defineProperty(exports, "__esModule", { value: true });
var httpie_1 = require("httpie");
var User_1 = __importDefault(require("../../src/models/User"));
var src_1 = require("../../src");
var FriendRequest_1 = __importDefault(require("../../src/models/FriendRequest"));
var FB_TEST_APP_ID = '353169041992501';
var FB_TEST_APP_TOKEN = '353169041992501|8d17708d062493030db44dd687b73e97';
function login(user) {
    return __awaiter(this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, user.save()];
                case 1: return [2 /*return*/, _a.sent()];
            }
        });
    });
}
exports.login = login;
function clearTestUsers() {
    return __awaiter(this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, User_1.default.deleteMany({})];
                case 1: return [2 /*return*/, _a.sent()];
            }
        });
    });
}
exports.clearTestUsers = clearTestUsers;
function clearFriendRequests() {
    return __awaiter(this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, FriendRequest_1.default.deleteMany({})];
                case 1: return [2 /*return*/, _a.sent()];
            }
        });
    });
}
exports.clearFriendRequests = clearFriendRequests;
var cachedAccessTokens;
function getTestUsersAccessTokens() {
    return __awaiter(this, void 0, void 0, function () {
        var res, response;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (!!cachedAccessTokens) return [3 /*break*/, 2];
                    return [4 /*yield*/, httpie_1.get("https://graph.facebook.com/v3.3/" + FB_TEST_APP_ID + "/accounts/test-users?access_token=" + FB_TEST_APP_TOKEN, {
                            headers: { 'Accept': 'application/json' }
                        })];
                case 1:
                    res = (_a.sent());
                    response = res.data;
                    if (response.error) {
                        throw new Error(response.error.message);
                    }
                    cachedAccessTokens = response.data.map(function (entry) { return entry.access_token; });
                    _a.label = 2;
                case 2: return [2 /*return*/, cachedAccessTokens];
            }
        });
    });
}
exports.getTestUsersAccessTokens = getTestUsersAccessTokens;
var cachedTestUsers;
function createFacebookTestUsers() {
    return __awaiter(this, void 0, void 0, function () {
        var accessTokens;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (!!cachedTestUsers) return [3 /*break*/, 3];
                    return [4 /*yield*/, getTestUsersAccessTokens()];
                case 1:
                    accessTokens = _a.sent();
                    return [4 /*yield*/, Promise.all(accessTokens.map(function (accessToken) {
                            return src_1.authenticate({ accessToken: accessToken });
                        }))];
                case 2:
                    cachedTestUsers = _a.sent();
                    _a.label = 3;
                case 3: return [2 /*return*/, cachedTestUsers];
            }
        });
    });
}
exports.createFacebookTestUsers = createFacebookTestUsers;
function includes(arr, targetId) {
    return arr.filter(function (id) { return id.toString() === targetId.toString(); }).length > 0;
}
exports.includes = includes;
