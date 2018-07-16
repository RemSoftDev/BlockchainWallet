import { Component, OnInit, HostListener } from '@angular/core';
import { AuthService } from './shared/services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(private authService: AuthService) { }

  ngOnInit() {
    if (localStorage.getItem('userName')) {
      this.authService.subscribuToNotifications();
    }

  }

  @HostListener('window:beforeunload', ['$event'])
  unsubscriveFromNotifications(event) {
    this.authService.unSubscribuFromNotifications();
  }
}
