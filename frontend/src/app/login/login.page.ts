import {Component, OnInit} from '@angular/core';
import {State} from 'src/state';
import {firstValueFrom, Subscription} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {ResponseDto, User} from 'src/Models';
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {environment} from 'src/environments/environment';
import {ToastController} from "@ionic/angular";
import {UserHandler} from '../userHandler';
import {TokenService} from 'src/TokenService';
import {ServerWelcomesUserDto} from "../../BaseDto";


@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
})
export class LoginPage implements OnInit {
  currentUser: User | undefined;
  //This is the formbuilder, it is important to SPELL the items as they are spelled in the dto in the API
  emailForm = new FormControl('', [Validators.required, Validators.minLength(2)])

  loginForm = new FormGroup(
    {
      email: this.emailForm
    }
  )

  private subToWelcomeUser: Subscription;
  private subscription: Subscription;
  dynamicLogInOutText: string = 'Login';


  constructor(public state: State, private tokenService: TokenService, private userHandler: UserHandler, public http: HttpClient, public fb: FormBuilder, public toastcontroller: ToastController) {
    this.subscription = this.userHandler.logInOutValue$.subscribe((value) => {
      this.dynamicLogInOutText = value;
    })
    this.subToWelcomeUser = this.state.welcomeUser$.subscribe((dto) => {
      this.presentToast(dto);
    });
  }

  ngOnInit() {
  }


  //check if we should login ot logout
  loginOut() {
    if (this.dynamicLogInOutText == 'Login') {
      this.login()
    } else {
      this.logout()
    }
  }

  login() {
    var object = {
      eventType: "ClientWantsToSignIn",
      Username: this.emailForm.value!
    }
    this.state.ws.send(JSON.stringify(object));
  }


  ServerWelcomesUser(dto: ServerWelcomesUserDto){
    console.log("Welcome")
    this.presentToast(dto);
  }

  private async presentToast(dto: ServerWelcomesUserDto) {
    this.toastcontroller.create({
      message: dto.message,
      duration: 5000,
      color: "success"
    }).then(r => r.present())
  }

  async logout() {
    this.tokenService.clearToken();

    (await this.toastcontroller.create({
      message: 'Successfully logged out',
      duration: 5000,
      color: 'success',
    })).present()

    this.userHandler.updateLoginOut("Login");
    this.userHandler.updateCurrentUser('');
  }

  changeNameOfCurrentUser(name: any): void {
    this.userHandler.updateCurrentUser(name);
  }

}
