import { Injectable } from "@angular/core";

@Injectable()
export class TokenService {
  //The token will be stored in the window storrage, so it stays on the user instead of the server
  private readonly storage: Storage = window.sessionStorage;

  setToken(token : string){
    this.storage.setItem("token", token);
    console.log(this.storage.getItem("token"))

  }

  getToken() {
    return this.storage.getItem("token");
  }

  clearToken() {
    this.storage.removeItem("token");
  }
}
