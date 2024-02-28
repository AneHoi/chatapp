import {Injectable} from "@angular/core";
import {User, Chat, ChatRoom, PeopleCounterDto, AllRoomsDto} from "./Models";
import {Subject} from "rxjs";
import {
  BaseDto,
  ServerBroardcastsMessageWithUsernameDto,
  ServerSendsErrorMessageToClient,
  ServerWelcomesUserDto
} from "./BaseDto";

@Injectable({
  providedIn: 'root'
})

export class State {

  ws: WebSocket = new WebSocket("ws://localhost:8181")
  allchatRooms: number[] = []
  chatsInRoom: Chat[] = []
  // this "{}" means: Empty object
  currentuser: User = {};

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

  welcomeUser = new Subject<ServerWelcomesUserDto>();
  welcomeUser$ = this.welcomeUser.asObservable();
  ServerWelcomesUser(dto: ServerWelcomesUserDto){
    this.welcomeUser.next(dto);
    console.log("Welcome");
  }

  errorMessage = new Subject<ServerSendsErrorMessageToClient>();
  errorMessage$ = this.errorMessage.asObservable();
  ServerSendsErrorMessageToClient(dto: ServerSendsErrorMessageToClient){
    this.errorMessage.next(dto)
  }

  peopleInChat = new Subject<PeopleCounterDto>();
  peopleInChat$ = this.peopleInChat.asObservable();
  PeopleCounter(dto: PeopleCounterDto){
    this.peopleInChat.next(dto)
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

  currentRoom = new Subject<number>();
  currentRoom$ = this.currentRoom.asObservable();

  setCuttentRoom(room: number): void{
    this.currentRoom.next(room);
  }


  chat = new Subject<ServerBroardcastsMessageWithUsernameDto>();
  chat$ = this.chat.asObservable();
  ServerBroardcastsMessageWithUsername(dto: ServerBroardcastsMessageWithUsernameDto){
    this.chat.next(dto)
  }
}
