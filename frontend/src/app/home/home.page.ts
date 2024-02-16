import {Component} from '@angular/core';
import {FormControl} from "@angular/forms";
import {BaseDto, ServerEchosClientDto, ServerSendsMessageBroadDto} from "../../BaseDto";
import {MessageDto, PeopleCounterDto} from "src/Models"

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
})
export class HomePage {
  chat: string | undefined;


  chatMessages: string[] = [];
  ws: WebSocket = new WebSocket("ws://localhost:8181")
  messageContent = new FormControl('');
  userName = new FormControl('');
  PeopleInChat: string = "1";

  constructor() {
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
    this.PeopleInChat = dto.numOfPeopleValue + "";
    this.chatMessages.push(dto.infoMessage)
    console.log(dto.numOfPeopleValue + "\n" + dto.infoMessage)
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
    this.ws.send(JSON.stringify(object));
  }

  sendMessageToMyself() {
    var object = {
      eventType: "ClientWansToEchoServer",
      messageContent: this.messageContent.value!
    }
    this.ws.send(JSON.stringify(object));
  }

  SignIn() {
    var object = {
      eventType: "ClientWantsToSignIn",
      Username: this.userName.value!
    }
    this.ws.send(JSON.stringify(object));

    var enterobject={
      eventType: "ClientWantsToEnterRoom",
      roomId: 2
    }
    this.ws.send(JSON.stringify(enterobject));
  }

  sendInChat() {
    var object = {
      eventType: "ClientWantsToSendToChatRoom",
      messageContent : this.messageContent.value,
      roomId : 2
    }
    this.ws.send(JSON.stringify(object));

  }
}
