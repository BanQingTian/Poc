"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result["default"] = mod;
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
const mongoose_1 = __importStar(require("mongoose"));
const RoomCacheSchema = new mongoose_1.Schema({
    clients: { type: Number, default: 0 },
    locked: { type: Boolean, default: false },
    maxClients: { type: Number, default: Infinity },
    metadata: mongoose_1.Schema.Types.Mixed,
    name: String,
    private: { type: Boolean, default: false },
    processId: String,
    roomId: String,
}, {
    strict: false,
    timestamps: true,
    versionKey: false,
});
RoomCacheSchema.index({ name: 1, locked: -1 });
RoomCacheSchema.index({ roomId: 1 });
const RoomCache = mongoose_1.default.model('RoomCache', RoomCacheSchema);
class MongooseDriver {
    constructor(connectionURI) {
        /* tslint:disable:no-string-literal */
        if (mongoose_1.default.connection.readyState === mongoose_1.default.connection.states['disconnected']) {
            /* tslint:enable:no-string-literal */
            mongoose_1.default.connect(connectionURI || process.env.MONGO_URI || 'mongodb://127.0.0.1:27017/colyseus', {
                autoIndex: true,
                useCreateIndex: true,
                useFindAndModify: true,
                useNewUrlParser: true,
                useUnifiedTopology: true,
            });
        }
    }
    createInstance(initialValues = {}) {
        return new RoomCache(initialValues);
    }
    find(conditions, additionalProjectionFields = {}) {
        return __awaiter(this, void 0, void 0, function* () {
            return (yield RoomCache.find(conditions, Object.assign({ _id: false, clients: true, createdAt: true, locked: true, maxClients: true, metadata: true, name: true, roomId: true }, additionalProjectionFields)));
        });
    }
    findOne(conditions) {
        return (RoomCache.findOne(conditions, {
            _id: 0,
            locked: 1,
            processId: 1,
            roomId: 1,
        }));
    }
}
exports.MongooseDriver = MongooseDriver;
