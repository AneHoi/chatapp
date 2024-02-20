import {Component} from '@angular/core';
import {FormControl} from "@angular/forms";
import {BaseDto, ServerEchosClientDto, ServerSendsMessageBroadDto} from "../../BaseDto";
import {MessageDto, PeopleCounterDto} from "src/Models"
import {environment} from "../../environments/environment";
import {State} from "../../state";

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
})
export class HomePage {
  chat: string | undefined;


  chatMessages: string[] = [];
  messageContent = new FormControl('');
  userName = new FormControl('');
  peopleInChat: string = "1";

  constructor(public state: State) {
  }

  PeopleCounter(dto: PeopleCounterDto){
    this.peopleInChat = dto.numOfPeopleValue + "";
    this.chatMessages.push(dto.infoMessage)
    console.log("people in chat from home" + dto.numOfPeopleValue + "\n" + dto.infoMessage)
  }

  ServerEchosClient(dto: ServerEchosClientDto) {
    this.chatMessages.push(dto.message!);
  }

  ServerSendsMessage(dto: ServerSendsMessageBroadDto) {
    this.chatMessages.push(dto.message!)
  }


  sendMessage() {
    var object = {
      eventType: "ClientWantsToBroardCast",
      messageContent: this.messageContent.value!
    }
    this.state.ws.send(JSON.stringify(object));
  }

  sendMessageToMyself() {
    var object = {
      eventType: "ClientWansToEchoServer",
      messageContent: this.messageContent.value!
    }
    this.state.ws.send(JSON.stringify(object));
  }

  SignIn() {
    var object = {
      eventType: "ClientWantsToSignIn",
      Username: this.userName.value!
    }
    this.state.ws.send(JSON.stringify(object));

    var enterobject={
      eventType: "ClientWantsToEnterRoom",
      roomId: 2
    }
    this.state.ws.send(JSON.stringify(enterobject));
  }

  sendInChat() {
    var object = {
      eventType: "ClientWantsToSendToChatRoom",
      messageContent : this.messageContent.value,
      roomId : 2
    }
    this.state.ws.send(JSON.stringify(object));

  }
}
