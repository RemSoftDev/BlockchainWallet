import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { BaseService } from "./base.service";

import { Observable } from 'rxjs/Rx';
import { BehaviorSubject } from 'rxjs/Rx';

import { HubConnectionBuilder }  from '@aspnet/signalr';
import { NotificationsService } from 'angular2-notifications';

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
  connection;

  constructor(private http: HttpClient, @Inject('BASE_URL') hostUrl: string, private _service: NotificationsService) {
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
        localStorage.setItem('userRoles', JSON.parse(JSON.stringify(res)).roles);
        localStorage.setItem('userName', JSON.parse(JSON.stringify(res)).userName);
        this.loggedIn = true;
        this._authNavStatusSource.next(true);
        this.subscribuToNotifications();
        return true;
      })
      .catch(this.handleError);
  }

  unSubscribuFromNotifications() {
    this.connection.invoke('Leave', localStorage.getItem('userName')).then(() => {
      this.connection.stop();
    });    
  }

  subscribuToNotifications() {

    this.connection = new HubConnectionBuilder()
      .withUrl('/notify')
      .build();

    this.connection.start()
      .then(() => {
        console.log('Connected');
        this.connection.invoke('Join', localStorage.getItem('userName'));
      });

    this.connection.on('Left', (data) => {
      console.log(data);
    });

    this.connection.on('Message', (payload: string) => {
      this._service.info('Notification',
        payload,
        {
          timeOut: 2000,
          showProgressBar: true,
          pauseOnHover: true,
          clickToClose: true
        });
    });
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
    this.unSubscribuFromNotifications();
    localStorage.removeItem('access_token');
    localStorage.removeItem('userRoles');
    localStorage.removeItem('userName');    
    this.loggedIn = false;
    this._authNavStatusSource.next(false);
  }

  isLoggedIn() {
    return this.loggedIn;
  }
}
