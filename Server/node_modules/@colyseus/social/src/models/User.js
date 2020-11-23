"use strict";
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result["default"] = mod;
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
var mongoose_1 = __importStar(require("mongoose"));
var Platform;
(function (Platform) {
    Platform["ios"] = "ios";
    Platform["android"] = "android";
})(Platform = exports.Platform || (exports.Platform = {}));
exports.UserExposedFields = ['username', 'displayName', 'avatarUrl', 'lang', 'location', 'timezone'];
var DeviceSchema = new mongoose_1.default.Schema({
    id: String,
    platform: String,
}, {
    _id: false
});
var UserSchema = new mongoose_1.Schema({
    username: { type: String, default: "" },
    displayName: { type: String, default: "" },
    avatarUrl: { type: String, default: "" },
    isAnonymous: { type: Boolean, default: true },
    email: { type: String },
    password: { type: String },
    passwordSalt: { type: String },
    lang: { type: String },
    location: { type: String },
    timezone: { type: String },
    metadata: { type: mongoose_1.Schema.Types.Mixed },
    devices: [DeviceSchema],
    facebookId: { type: String },
    twitterId: { type: String },
    googleId: { type: String },
    gameCenterId: { type: String },
    steamId: { type: String },
    friendIds: { type: [mongoose_1.Schema.Types.ObjectId], default: [] },
    blockedUserIds: { type: [mongoose_1.Schema.Types.ObjectId], default: [] },
}, {
    timestamps: true
});
// TODO:
// UserSchema.indexes
exports.default = mongoose_1.default.model('User', UserSchema);
