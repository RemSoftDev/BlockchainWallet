import { Component, HostListener } from '@angular/core';
import { NotificationsService } from './shared/services/notifications.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(private notificationsService: NotificationsService) { }

  @HostListener('window:beforeunload', ['$event'])
  unsubscriveFromNotifications(event) {
    this.notificationsService.unSubscribuFromNotifications();
  }
}
