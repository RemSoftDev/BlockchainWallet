import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';

@Injectable()
export class AuthUserGuardService implements CanActivate {

  constructor(public router: Router) { }

  canActivate(): boolean {
    if (!localStorage.getItem('access_token')) {
      this.router.navigate(['/']);
      return false;
    }
    return true;
  }
}
