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



  constructor(public state: State, public http: HttpClient) {
    this.state.listener()
  }


  AllChatMessagesInRoom(dto: ChatsInRoomDTO){
    for (let msg of dto) {
      this.state.chatsInRoom.push(msg);
    }
  }


  openChat(chat: number) {
    this.state.setCuttentRoom(chat);
    console.log("You tapped into chat: " + chat);
  }
}
