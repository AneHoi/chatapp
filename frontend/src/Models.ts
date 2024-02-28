
export interface MessageDto {
  eventType: string
  messageContent: string
}
export interface PeopleCounterDto{
  numOfPeopleValue: number
  infoMessage: string
}

export interface AllRoomsDto{
  roomIds: number []
}

export type ChatRoomDTO = ChatRoom[]
export interface ChatRoom {
  id: number
  name: string
}


export type ChatsInRoomDTO = Chat[]
export interface Chat{
  id: number
  roomId: number
  sender: string
  message: string
}

export class ResponseDto<T> {
  responseData?: T;
  messageToClient?: string;
}

export class User {
  username?: string;
  tlfnumber?: number;
  email?: string;
  password?: string;
}

