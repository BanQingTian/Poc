import { getLoger } from "./logs/logger"
var logger = getLoger("DataBase");


export class mongodClient {
    client = require('mongodb').MongoClient;

    public async create(collection: string): Promise<boolean> {
        let promise = new Promise<boolean>((resolve, reject) => {
            this.client.connect(process.env.MONGO_URI, { useNewUrlParser: true, useUnifiedTopology: true }, function (err, db) {
                if (err) throw err;
                var dbase = db.db("admin");
                dbase.createCollection(collection, function (err, res) {
                    if (err) {
                        resolve(false);
                        throw err;
                    }
                    logger.info("Create a collection:" + collection);
                    db.close();
                    resolve(true);
                });
            });
        });

        return promise;
    }

    public async insertOrUpdate(collection: string, whereStr: {}, data: Map<string, any>): Promise<boolean> {
        let promise: Promise<boolean> = new Promise<boolean>((resolve, reject) => {
            this.client.connect(process.env.MONGO_URI, { useNewUrlParser: true, useUnifiedTopology: true }, function (err, db) {
                if (err) throw err;
                var dbase = db.db("admin");
                dbase.collection(collection).updateOne(whereStr, { $set: data }, { upsert: true }, function (err, res) {
                    if (err) {
                        resolve(false);
                        throw err;
                    }
                    logger.info("Insert a doc success!");
                    db.close();
                    resolve(true);
                });
            });
        });
        return promise;
    }

    public async find(collection: string, whereStr: any): Promise<any> {
        let promise = new Promise<any>((resolve, reject) => {
            this.client.connect(process.env.MONGO_URI, { useNewUrlParser: true, useUnifiedTopology: true }, function (err, db) {
                if (err) throw err;
                var dbase = db.db("admin");
                let col = dbase.collection(collection);
                col.find(whereStr).toArray(function (err, result) {
                    if (err) throw err;
                    db.close();
                    resolve(result);
                });;
            });
        });
        return promise;
    }
}