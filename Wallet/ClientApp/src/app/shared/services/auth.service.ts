import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { BaseService } from "./base.service";

import { Observable } from 'rxjs/Rx';
import { BehaviorSubject } from 'rxjs/Rx';

import 'rxjs/add/operator/map';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
  })
};

@Injectable()
export class AuthService extends BaseService {

  hostUrl: string;
  baseUrl: string = "api/Account";
  private _authNavStatusSource = new BehaviorSubject<boolean>(false);
  authNavStatus$ = this._authNavStatusSource.asObservable();
  private loggedIn = false;

  constructor(private http: HttpClient, @Inject('BASE_URL') hostUrl: string) {
    super();
    this.loggedIn = !!localStorage.getItem('access_token');
    this._authNavStatusSource.next(this.loggedIn);
    this.hostUrl = hostUrl;
  }
  
  resetPass(email, password, code) {
    return this.http
      .post(
       this.hostUrl + this.baseUrl + '/ResetPassword',
        JSON.stringify({ email, password, code }),
        httpOptions)
      .map(res => {
        return true;
      })
      .catch(this.handleError);
  }

  confirmEmail(userId, code) {
    return this.http
      .post(
        this.hostUrl + this.baseUrl + '/ConfirmEmail',
        JSON.stringify({ userId, code }),
        httpOptions)
      .map(res => {
        return true;
      })
      .catch(this.handleError);
  }

  forgotPass(email) {
    return this.http
      .post(
        this.hostUrl + this.baseUrl + '/ForgotPassword',
        JSON.stringify({ email }),
        httpOptions)
      .map(res => {
        return true;
      })
      .catch(this.handleError);
  }

  login(email, password) {
    return this.http
      .post(
        this.hostUrl + this.baseUrl + '/Login',
        JSON.stringify({ email, password }),
        httpOptions)
      .map(res => {
        localStorage.setItem('access_token', JSON.parse(JSON.stringify(res)).access_token);
        this.loggedIn = true;
        this._authNavStatusSource.next(true);
        return true;
      })
      .catch(this.handleError);
  }

  signIn(email, password, passwordConfirm) {
    return this.http
      .post(
        this.hostUrl + this.baseUrl + '/Register',
        JSON.stringify({ email, password, passwordConfirm }),
        httpOptions)
      .map(res => {
        return true;
      })
      .catch(this.handleError);
  }

  logout() {
    localStorage.removeItem('access_token');
    this.loggedIn = false;
    this._authNavStatusSource.next(false);
  }

  isLoggedIn() {
    return this.loggedIn;
  }
}
