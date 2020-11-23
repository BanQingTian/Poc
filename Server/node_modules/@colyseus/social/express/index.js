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
var express_1 = __importDefault(require("express"));
var express_jwt_1 = __importDefault(require("express-jwt"));
var src_1 = require("../src");
var User_1 = __importDefault(require("../src/models/User"));
var env_1 = require("../src/env");
var auth_1 = require("../src/auth");
var push_notifications_1 = require("../src/push_notifications");
var WebPushSubscription_1 = __importDefault(require("../src/models/WebPushSubscription"));
var tryOrErr = function (res, cb, statusCode) { return __awaiter(_this, void 0, void 0, function () {
    var e_1;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                _a.trys.push([0, 2, , 3]);
                return [4 /*yield*/, cb()];
            case 1:
                _a.sent();
                return [3 /*break*/, 3];
            case 2:
                e_1 = _a.sent();
                console.error(e_1.message);
                res.status(statusCode);
                res.json({ error: (e_1.data && e_1.data.error && e_1.data.error.message) || e_1.message });
                return [3 /*break*/, 3];
            case 3: return [2 /*return*/];
        }
    });
}); };
var jwtMiddleware = express_jwt_1.default({
    secret: env_1.JWT_SECRET,
    userProperty: "auth",
    getToken: function (req) {
        if (req.headers.authorization && req.headers.authorization.split(' ')[0] === 'Bearer') {
            return req.headers.authorization.split(' ')[1];
        }
        else if (req.query && req.query.token) {
            return req.query.token;
        }
        return null;
    }
});
exports.jwtMiddleware = jwtMiddleware;
// connect into the database!
src_1.connectDatabase();
/**
 * Auth Routes
 */
var auth = express_1.default.Router();
auth.post("/", function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            var _a, accessToken, deviceId, platform, token, email, password, user;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        _a = req.query, accessToken = _a.accessToken, deviceId = _a.deviceId, platform = _a.platform, token = _a.token, email = _a.email, password = _a.password;
                        return [4 /*yield*/, src_1.authenticate({ accessToken: accessToken, deviceId: deviceId, platform: platform, token: token, email: email, password: password })];
                    case 1:
                        user = _b.sent();
                        if (!(deviceId && platform)) return [3 /*break*/, 3];
                        return [4 /*yield*/, src_1.assignDeviceToUser(user, deviceId, platform)];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3:
                        res.json(__assign({}, user.toJSON(), auth_1.createToken(user)));
                        return [2 /*return*/];
                }
            });
        }); }, 401);
        return [2 /*return*/];
    });
}); });
auth.put("/", jwtMiddleware, express_1.default.json(), function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            var _a, _b, _c;
            return __generator(this, function (_d) {
                switch (_d.label) {
                    case 0:
                        _b = (_a = res).json;
                        _c = {};
                        return [4 /*yield*/, src_1.updateUser(req.auth._id, req.body)];
                    case 1:
                        _b.apply(_a, [(_c.status = _d.sent(), _c)]);
                        return [2 /*return*/];
                }
            });
        }); }, 500);
        return [2 /*return*/];
    });
}); });
auth.get("/", jwtMiddleware, function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            var status, _a, _b, _c;
            return __generator(this, function (_d) {
                switch (_d.label) {
                    case 0:
                        status = req.query.status;
                        _b = (_a = res).json;
                        _c = {};
                        return [4 /*yield*/, src_1.pingUser(req.auth._id)];
                    case 1:
                        _b.apply(_a, [(_c.status = _d.sent(), _c)]);
                        return [2 /*return*/];
                }
            });
        }); }, 500);
        return [2 /*return*/];
    });
}); });
var friend = express_1.default.Router();
friend.use(jwtMiddleware);
friend.get("/requests", function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            var requests, users;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, src_1.getFriendRequests(req.auth._id)];
                    case 1:
                        requests = _a.sent();
                        return [4 /*yield*/, src_1.getFriendRequestsProfile(requests)];
                    case 2:
                        users = _a.sent();
                        res.json(users);
                        return [2 /*return*/];
                }
            });
        }); }, 500);
        return [2 /*return*/];
    });
}); });
friend.put("/requests", function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, src_1.consumeFriendRequest(req.auth._id, req.params.userId)];
                    case 1:
                        _a.sent();
                        res.json({ success: true });
                        return [2 /*return*/];
                }
            });
        }); }, 500);
        return [2 /*return*/];
    });
}); });
friend.delete("/requests", function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, src_1.consumeFriendRequest(req.auth._id, req.params.userId, false)];
                    case 1:
                        _a.sent();
                        res.json({ success: true });
                        return [2 /*return*/];
                }
            });
        }); }, 500);
        return [2 /*return*/];
    });
}); });
friend.post("/requests", function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, src_1.sendFriendRequest(req.auth._id, req.params.userId)];
                    case 1:
                        _a.sent();
                        res.json({ success: true });
                        return [2 /*return*/];
                }
            });
        }); }, 500);
        return [2 /*return*/];
    });
}); });
friend.get("/all", function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            var user, _a, _b;
            return __generator(this, function (_c) {
                switch (_c.label) {
                    case 0: return [4 /*yield*/, User_1.default.findOne({ _id: req.auth._id })];
                    case 1:
                        user = _c.sent();
                        _b = (_a = res).json;
                        return [4 /*yield*/, src_1.getFriends(user)];
                    case 2:
                        _b.apply(_a, [_c.sent()]);
                        return [2 /*return*/];
                }
            });
        }); }, 500);
        return [2 /*return*/];
    });
}); });
friend.get("/online", function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            var user, _a, _b;
            return __generator(this, function (_c) {
                switch (_c.label) {
                    case 0: return [4 /*yield*/, User_1.default.findOne({ _id: req.auth._id })];
                    case 1:
                        user = _c.sent();
                        _b = (_a = res).json;
                        return [4 /*yield*/, src_1.getOnlineFriends(user)];
                    case 2:
                        _b.apply(_a, [_c.sent()]);
                        return [2 /*return*/];
                }
            });
        }); }, 500);
        return [2 /*return*/];
    });
}); });
friend.post("/block", function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                src_1.blockUser(req.auth._id, req.query.userId);
                res.json({ success: true });
                return [2 /*return*/];
            });
        }); }, 500);
        return [2 /*return*/];
    });
}); });
friend.put("/block", function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                src_1.unblockUser(req.auth._id, req.query.userId);
                res.json({ success: true });
                return [2 /*return*/];
            });
        }); }, 500);
        return [2 /*return*/];
    });
}); });
/**
 * Push Notification Routes
 */
var push = express_1.default.Router();
push.get("/", function (req, res) {
    // TODO: Cache this URL?
    res.set("Content-Type", "application/x-javascript");
    res.send(push_notifications_1.ServiceWorkerScript.replace("[BACKEND_URL]", req.protocol + '://' + req.get('host') + req.originalUrl));
});
// send push notifications to all subscribers
push.post("/", function (_, res) { return __awaiter(_this, void 0, void 0, function () {
    var results;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4 /*yield*/, push_notifications_1.sendNotification({
                    title: "Title, it works!",
                    body: "Hello, body!",
                })];
            case 1:
                results = _a.sent();
                res.json(results);
                return [2 /*return*/];
        }
    });
}); });
// expose web push public key
push.get("/web", function (_, res) { return res.json({ publicKey: process.env.WEBPUSH_PUBLIC_KEY }); });
// store user subscription
push.post("/subscribe", function (req, res) { return __awaiter(_this, void 0, void 0, function () {
    var _this = this;
    return __generator(this, function (_a) {
        tryOrErr(res, function () { return __awaiter(_this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, WebPushSubscription_1.default.create(req.body)];
                    case 1:
                        _a.sent();
                        res.json({ success: true });
                        return [2 /*return*/];
                }
            });
        }); }, 500);
        return [2 /*return*/];
    });
}); });
var routes = express_1.default.Router();
routes.use("/auth", auth);
routes.use("/push", express_1.default.json(), push);
routes.use("/friends", friend);
exports.default = routes;
