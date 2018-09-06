import { Injectable } from '@angular/core';
import { HubConnectionBuilder } from '@aspnet/signalr';
import { BehaviorSubject } from 'rxjs/Rx';
import { NotificationsService } from 'angular2-notifications';

@Injectable()
export class NotificationService {

  constructor(private _service: NotificationsService) {}

  receivedData;
  connection;
  private _receivingStatusSource = new BehaviorSubject<boolean>(false);
  receivingNavStatus$ = this._receivingStatusSource.asObservable();

  getData() {
    return this.receivedData;
  }

  unSubscribuFromNotifications() {
    if (this.connection) {
      this.connection.invoke('Leave', localStorage.getItem('userName')).then(() => {
        this.connection.stop();
      }); 
    }   
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
      console.log(this.receivedData);

      if (this.shouldMakeSound()) {
        this.playSound();
        this._service.info("Notification",
          "New Transaction",
          {
            timeOut: 2000,
            showProgressBar: true,
            pauseOnHover: true,
            clickToClose: true
          });
      }

      this._receivingStatusSource.next(true);    
    });
  }

  playSound() {
    let audio = new Audio();
    audio.src = "../../../assets/notification.mp3";
    audio.load();
    audio.play();
  }

  shouldMakeSound() {
    if (this.receivedData) {
      for (let entry of this.receivedData) {
        if (entry.account.isNotificated || entry.contract.isNotificated) {
          return true;
        }
      }
      return false;
    }
  }

}
