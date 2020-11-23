import mongoose, { Document } from 'mongoose';
export interface IWebPushSubscription extends Document {
    endpoint: string;
    expirationTime: Date;
    keys: {
        p256dh: string;
        auth: string;
    };
}
declare const _default: mongoose.Model<IWebPushSubscription, {}>;
export default _default;
