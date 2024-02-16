import {Component} from '@angular/core';
import {Chat} from "../Models";

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
})
export class AppComponent {
  chatname: string = 'placeholder';
  allchates = [
    {id: 1, name: 'Studygroup'},
    {id: 2, name: 'Moviegroup'}
  ];


  constructor() {
  }

  openChat() {
    console.log("Tapped into chat")

  }
}
