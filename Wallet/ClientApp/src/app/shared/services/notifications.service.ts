import { Injectable } from '@angular/core';
import { HubConnectionBuilder } from '@aspnet/signalr';
import { BehaviorSubject } from 'rxjs/Rx';

@Injectable()
export class NotificationsService {

  receivedData;
  connection;
  private _receivingStatusSource = new BehaviorSubject<boolean>(false);
  receivingNavStatus$ = this._receivingStatusSource.asObservable();

  getData() {
    return this.receivedData;
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
      this.receivedData = payload;
      this._receivingStatusSource.next(true);    
    });
  }
}
