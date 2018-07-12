import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../../shared/services/auth.service';
import { RedirectionService } from '../../shared/services/redirection.service';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit, OnDestroy {

  status: boolean;
  isWatchlistPage: boolean;
  subscription: Subscription;
  pagesubscription: Subscription;
  isAdmin:boolean;

  constructor(private authService: AuthService, private redirectionService: RedirectionService ) { }

  logout() {
    this.authService.logout();
  }

  checkUserRole() {
    if (localStorage.getItem('userRoles') === "ApiAdmin") {
      this.isAdmin = true;
    } else {
      this.isAdmin = false;
    };
  }

  ngOnInit() {
    this.subscription = this.authService.authNavStatus$.subscribe(status => {
        this.status = status;
        this.checkUserRole();
      }
    );
    this.pagesubscription =
      this.redirectionService.redirectionhNavStatus$.subscribe(redirected => this.isWatchlistPage = redirected);
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.pagesubscription.unsubscribe();
  }

}
