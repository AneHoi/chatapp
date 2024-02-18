import {Injectable} from "@angular/core";
import {User, Chat, ChatRoom} from "./Models";
import {Subject} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class State {
  allchatRooms: ChatRoom[] = []
  chatsInRoom: Chat[] = []
  // this "{}" means: Empty object
  currentuser: User = {};


  getCurrentUser(): User {
    return this.currentuser;
  }

  setCurrentUser(user: User): void {
    this.currentuser = user;
  }
}



