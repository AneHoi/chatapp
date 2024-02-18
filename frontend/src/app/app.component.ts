import {Component} from '@angular/core';
import {Chat, ChatsInRoomDTO, ChatRoomDTO} from "../Models";
import {State} from "../state";
import {firstValueFrom} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {environment} from "../environments/environment";
import {BaseDto} from "../BaseDto";

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
})
export class AppComponent {


  ws: WebSocket = new WebSocket("ws://localhost:8181")

  constructor(public state: State, public http: HttpClient) {
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

  AllRooms(dto: ChatRoomDTO){
    this.state.allchatRooms = dto
  }

  AllChatMessagesInRoom(dto: ChatsInRoomDTO){
    for (let msg of dto) {
      this.state.chatsInRoom.push(msg);
    }
  }

  openChat() {
    console.log("Tapped into chat")

  }
}
