import mongoose, { Schema, Document } from 'mongoose';
export interface IFriendRequest extends Document {
    sender: Schema.Types.ObjectId;
    receiver: Schema.Types.ObjectId;
}
declare const _default: mongoose.Model<IFriendRequest, {}>;
export default _default;
