import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../../shared/services/auth.service';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit, OnDestroy {

  status: boolean;
  subscription: Subscription;

  constructor(private authService: AuthService) { }

  logout() {
    this.authService.logout();
  }

  ngOnInit() {
    this.subscription = this.authService.authNavStatus$.subscribe(status => this.status = status);
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

}
