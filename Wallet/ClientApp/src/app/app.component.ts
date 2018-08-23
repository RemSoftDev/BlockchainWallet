import { Component, HostListener, OnInit } from '@angular/core';
import { NotificationsService } from './shared/services/notifications.service';
import { AuthService } from './shared/services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  ngOnInit() {
    this.authService.checkTokenExpired();
  }

  constructor(private notificationsService: NotificationsService, private authService: AuthService) { }

  @HostListener('window:beforeunload', ['$event'])
  unsubscriveFromNotifications(event) {
    this.notificationsService.unSubscribuFromNotifications();
  }
}
