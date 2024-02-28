import {Component} from '@angular/core';
import {FormControl} from "@angular/forms";
import {
  BaseDto,
  ServerEchosClientDto,
  ServerSendsErrorMessageToClient,
  ServerSendsMessageBroadDto,
  ServerWelcomesUserDto
} from "../../BaseDto";
import {MessageDto, PeopleCounterDto} from "src/Models"
import {environment} from "../../environments/environment";
import {State} from "../../state";
import {Subscription} from "rxjs";
import {ToastController} from "@ionic/angular";

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
  peopleInChatSub: Subscription;
  peopleInChat: string = "";
  chatroomName: string = "No chatrooms selected";
  currentRoomSub: Subscription;
  chatroomNameSub: Subscription;
  failsTologIn: Subscription;
  chatMessagesSub: Subscription;
  selectedChatRoom: number = 0;

  constructor(public state: State, public toastcontroller: ToastController) {
    this.failsTologIn = this.state.errorMessage$.subscribe((dto) => {
      this.presentToast(dto);
    });
    this.peopleInChatSub = this.state.peopleInChat$.subscribe((dto) => {
      this.peopleInChat = dto.numOfPeopleValue + "";
    });
    this.chatroomNameSub = this.state.currentRoom$.subscribe((dto) => {
      this.chatroomName = "Chatroom: " + dto + "";
      var object = {
        eventType: "ClientWantsToEnterRoom",
        roomId: dto,
      }
      this.state.ws.send(JSON.stringify(object));
    });
    this.chatMessagesSub = this.state.chat$.subscribe((dto) => {
      this.chatMessages.push(dto.username + ": " + dto.message);
    });
    this.currentRoomSub = this.state.currentRoom$.subscribe((dto) => {
      this.selectedChatRoom = dto
      this.chatMessages = []
    });
  }
  private async presentToast(dto: ServerSendsErrorMessageToClient) {
    this.toastcontroller.create({
      message: dto.errorMessage,
      duration: 5000,
      color: "danger"
    }).then(r => r.present())
  }


  ServerEchosClient(dto: ServerEchosClientDto) {
    this.chatMessages.push(dto.message!);
  }

  ServerSendsMessage(dto: ServerSendsMessageBroadDto) {
    this.chatMessages.push(dto.message!)
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
  }

  sendInChat() {
    var object = {
      eventType: "ClientWantsToSendToChatRoom",
      messageContent : this.messageContent.value,
      roomId : this.selectedChatRoom
    }
    this.state.ws.send(JSON.stringify(object));

  }
}
