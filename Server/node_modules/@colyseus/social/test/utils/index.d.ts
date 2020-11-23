/// <reference types="mongodb" />
import { IUser } from "../../src/models/User";
import { ObjectId } from "../../src";
export declare function login(user: IUser): Promise<IUser>;
export declare function clearTestUsers(): Promise<{
    ok?: number;
    n?: number;
}>;
export declare function clearFriendRequests(): Promise<{
    ok?: number;
    n?: number;
}>;
export declare function getTestUsersAccessTokens(): Promise<string[]>;
export declare function createFacebookTestUsers(): Promise<IUser[]>;
export declare function includes(arr: ObjectId[], targetId: ObjectId): boolean;
