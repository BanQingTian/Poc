import { buryingPointTool } from "./buryingDataPoint"

process.env.MONGO_URI = "mongodb://root:nreal123456@192.168.69.60:27017/admin";

console.log(Date.now());
var tool = new buryingPointTool("test001");
tool.log(process.env.CreatRoomID, new Map<string, any>([
    ['name', 'xiaoming1'],
    ['age', 12]
]));
tool.log(process.env.JoinRoomID, new Map<string, any>([
    ['name', 'xiaoming2'],
    ['age', 12]
]));
tool.log(process.env.StartGameID, new Map<string, any>([
    ['name', 'xiaoming3'],
    ['age', 12]
]));
tool.log(process.env.QuitGameID, new Map<string, any>([
    ['name', 'xiaoming4'],
    ['age', 12]
]));

tool.save();