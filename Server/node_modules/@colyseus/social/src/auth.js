"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
var jsonwebtoken_1 = __importDefault(require("jsonwebtoken"));
var crypto_1 = __importDefault(require("crypto"));
var env_1 = require("./env");
function hashPassword(password) {
    // creating a unique salt for a particular user
    var salt = crypto_1.default.randomBytes(16).toString('hex');
    // hashing user's salt and password with 1000 iterations, 32 length and sha512 digest
    var hash = crypto_1.default.pbkdf2Sync(password, salt, 1000, 32, "sha512").toString("hex");
    return { salt: salt, hash: hash };
}
exports.hashPassword = hashPassword;
function isValidPassword(user, password) {
    var hash = crypto_1.default.pbkdf2Sync(password, user.passwordSalt, 1000, 32, "sha512").toString("hex");
    return user.password === hash;
}
exports.isValidPassword = isValidPassword;
function createToken(user) {
    return { token: jsonwebtoken_1.default.sign({ _id: user._id }, env_1.JWT_SECRET) };
    // const expiresIn = 60 * 60; // an hour
    // const data = { _id: user._id };
    // return {
    //     expiresIn,
    //     token: jwt.sign(data, JWT_SECRET, { expiresIn })
    // };
}
exports.createToken = createToken;
function verifyToken(token) {
    return jsonwebtoken_1.default.verify(token, env_1.JWT_SECRET);
}
exports.verifyToken = verifyToken;
