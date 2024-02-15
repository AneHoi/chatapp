import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {RouterModule, Routes} from '@angular/router';

import {IonicModule} from '@ionic/angular';

import {AppComponent} from './app.component';
import {HeaderComponent} from "./header/header.component";
import {LoginPage} from "./login/login.page";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {HomePage} from "./home/home.page";
import {RegisterComponent} from "./register/register.component";
import {HTTP_INTERCEPTORS, provideHttpClient, withJsonpSupport} from "@angular/common/http";
import {AuthHttpInterceptor} from "../interceptors/auth-http-interceptor";
import {TokenService} from "../TokenService";
import {CommonModule} from "@angular/common";
import {ChatComponent} from "./chat/chat.component";

const routes: Routes = [
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {component: HomePage, path: 'home'},
  {component: LoginPage, path: 'login'},
  {component: RegisterComponent, path: 'register'}
];

//It is very important to remember ALL the different pages here!!!
@NgModule({
  declarations: [AppComponent, HeaderComponent, LoginPage, RegisterComponent, HomePage, RegisterComponent, ChatComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [
    RouterModule.forRoot(routes),
    BrowserModule,
    IonicModule.forRoot(),
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: AuthHttpInterceptor, multi: true},
    provideHttpClient(withJsonpSupport()), TokenService
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
}
