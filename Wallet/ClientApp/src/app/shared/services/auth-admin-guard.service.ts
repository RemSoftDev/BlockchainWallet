import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';

@Injectable()
export class AuthAdminGuardService implements CanActivate {

  constructor(public router: Router) { }

  canActivate(): boolean {
    if (!(localStorage.getItem('userRoles') === "ApiAdmin")) {
      this.router.navigate(['/']);
      return false;
    }
    return true;
  }
}
