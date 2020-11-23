"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
process.env.WEBPUSH_SUBJECT = 'http://localhost:8000';
process.env.WEBPUSH_PUBLIC_KEY = 'BN1oq1kO-MZFcexPov2yNLSit3R3KcElnH1wJiuXqA3p6V96vK7_LC1JpmqDNOQxf-6umOk1Yl0N8lFiQO6mdd8';
process.env.WEBPUSH_PRIVATE_KEY = '7nIUJ1rZ5fcpy_bDHjFWEFHW-HZJ3CMms-G94a9Upsg';
var cors_1 = __importDefault(require("cors"));
var express_1 = __importDefault(require("express"));
var express_2 = __importDefault(require("../express"));
var port = 3000;
var app = express_1.default();
app.use(cors_1.default());
app.use("/", express_2.default);
app.listen(port);
console.log("Listening on http://localhost:" + port);
