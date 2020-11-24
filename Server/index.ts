import http from "http";
import express from "express";
import cors from "cors";

import { getLoger } from "./logs/logger"
var logger = getLoger("StartUp");

let PORT: number = 2567;
var pjson = require('./package.json');
logger.info("process.env.NODE_ENV:" + process.env.NODE_ENV);
if (process.env.NODE_ENV == 'prod') {
  process.env.MONGO_URI = pjson.env_prod.MONGO_URI;
  PORT = Number(pjson.env_prod.PORT || 2567)
} else if (process.env.NODE_ENV == 'uat') {
  process.env.MONGO_URI = pjson.env_uat.MONGO_URI;
  PORT = Number(pjson.env_uat.PORT || 2567)
} else if (process.env.NODE_ENV == 'test') {
  process.env.MONGO_URI = pjson.env_test.MONGO_URI;
  PORT = Number(pjson.env_test.PORT || 2567)
}else if (process.env.NODE_ENV == 'cz') {
    process.env.MONGO_URI = pjson.env_cz.MONGO_URI;
    PORT = Number(pjson.env_test.PORT || 2567)
} else if (process.env.NODE_ENV == 'foreign') {
  process.env.MONGO_URI = pjson.env_foreign.MONGO_URI;
  PORT = Number(pjson.env_foreign.PORT || 2567)
}

import { Server, FossilDeltaSerializer } from "colyseus";
import { NrealRoom } from "./NrealRoom";
import socialRoutes from "@colyseus/social/express";

import { ConnectResult } from "./Entity";
import { MongooseDriver } from "colyseus/lib/matchmaker/drivers/MongooseDriver"

const app = express();

/**
 * CORS should be used during development only.
 * Please remove CORS on production, unless you're hosting the server and client on different domains.
 */
// app.use(cors());

const gameServer = new Server({
  server: http.createServer(app),
  pingInterval: 0,
  driver: new MongooseDriver(),
});

// Register Nreal as "NrealRoom"
gameServer.define("Nreal", NrealRoom);
app.use("/", socialRoutes);

//create a server object:
http.createServer(function (req, res) {
  var msg = new ConnectResult();
  msg.result = true;
  msg.version = pjson.version;
  res.write(JSON.stringify(msg)); //write a response to the client
  res.end(); //end the response
}).listen(PORT + 1);

// Listen on specified PORT number
logger.info(`Running on ws://localhost:${PORT}`);
gameServer.listen(PORT);
logger.info("Welcome to the Fairy Tower Game!")
