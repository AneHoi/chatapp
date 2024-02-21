import {Injectable} from "@angular/core";
import {User, Chat, ChatRoom, PeopleCounterDto, AllRoomsDto} from "./Models";
import {Subject} from "rxjs";
import {BaseDto} from "./BaseDto";

@Injectable({
  providedIn: 'root'
})

export class State {

  ws: WebSocket = new WebSocket("ws://localhost:8181")
  allchatRooms: number[] = []
  chatsInRoom: Chat[] = []
  // this "{}" means: Empty object
  currentuser: User = {};
  peopleInChat: string = "1";
  chatMessages: string[] = [];
  currentChatRoom: ChatRoom | undefined


  listener() {
    this.ws.onmessage = message => {
      try {
        const messageFromServer = JSON.parse(message.data) as BaseDto<any>
        // @ts-ignore
        this[messageFromServer.eventType].call(this, messageFromServer);
      } catch (exeption) {
        // @ts-ignore
        this.chatMessages.push(message.data)
      }
    }
  }

  PeopleCounter(dto: PeopleCounterDto){
    this.peopleInChat = dto.numOfPeopleValue + "";
    this.chatMessages.push(dto.infoMessage)
    console.log("people in chats: " + dto.numOfPeopleValue + "\n" + dto.infoMessage)
  }

  AllRooms(dto: AllRoomsDto){
    this.allchatRooms = dto.roomIds;
    console.log("Rooms: " + dto.roomIds)
  }

  getCurrentUser(): User {
    return this.currentuser;
  }

  setCurrentUser(user: User): void {
    this.currentuser = user;
  }
}



