import { Component, HostListener, OnInit } from '@angular/core';
import { NotificationService } from './shared/services/notifications.service';
import { AuthService } from './shared/services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  ngOnInit() {
    this.authService.checkTokenExpired();
    if (localStorage.getItem('access_token')) {
      this.notificationsService.subscribuToNotifications();
    }
  }

  constructor(private notificationsService: NotificationService, private authService: AuthService) { }

  @HostListener('window:beforeunload', ['$event'])
  unsubscriveFromNotifications(event) {
    if (localStorage.getItem('access_token')) {
      this.notificationsService.unSubscribuFromNotifications();
    }
  }
}
