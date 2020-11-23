import { IUser } from "./models/User";
export interface TokenData {
    token: string;
}
export interface AuthDataInToken {
    _id: string;
}
export declare function hashPassword(password: string): {
    salt: string;
    hash: string;
};
export declare function isValidPassword(user: IUser, password: any): boolean;
export declare function createToken(user: IUser): TokenData;
export declare function verifyToken(token: string): AuthDataInToken;
