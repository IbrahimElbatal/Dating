export interface Message{
    id : number,
    content:string,
    senderId :number,
    recipientId :number,
    senderKnownAs :string,
    recipientKnownAs :string,
    senderPhotoUrl :string,
    recipientPhotoUrl:string,
    isRead :boolean,
    dateSent:Date

}