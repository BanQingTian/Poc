import mongoose from "mongoose";
import User, { IUser, Platform } from "./models/User";
import { MongoError } from "mongodb";
import FriendRequest, { IFriendRequest } from "./models/FriendRequest";
import { verifyToken } from "./auth";
export declare type ObjectId = string | mongoose.Schema.Types.ObjectId;
export declare function connectDatabase(cb?: (err: MongoError) => void): Promise<void>;
export declare function pingUser(userId: ObjectId): Promise<boolean>;
export declare function authenticate({ accessToken, deviceId, platform, email, password, token }: {
    accessToken?: string;
    deviceId?: string;
    platform?: string;
    email?: string;
    password?: string;
    token?: string;
}): Promise<IUser>;
export declare function updateUser(_id: ObjectId, fields: {
    [id in keyof IUser]: any;
}): Promise<boolean>;
export declare function assignDeviceToUser(user: IUser, deviceId: string, platform: Platform): Promise<void>;
export declare function sendFriendRequest(senderId: ObjectId, receiverId: ObjectId): Promise<any>;
export declare function consumeFriendRequest(receiverId: ObjectId, senderId: ObjectId, accept?: boolean): Promise<void>;
export declare function blockUser(userId: ObjectId, blockedUserId: ObjectId): Promise<void>;
export declare function unblockUser(userId: ObjectId, blockedUserId: ObjectId): Promise<void>;
export declare function getFriendRequests(userId: ObjectId): Promise<IFriendRequest[]>;
export declare function getFriendRequestsProfile(friendRequests: IFriendRequest[], fields?: Array<keyof IUser>): Promise<IUser[]>;
export declare function getFriends(user: IUser, fields?: Array<keyof IUser>): Promise<IUser[]>;
export declare function getOnlineFriends(user: IUser, fields?: Array<keyof IUser>): Promise<IUser[]>;
export { verifyToken, FriendRequest, IFriendRequest, User, IUser, mongoose };
