import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../shared/services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
})
export class SigninComponent {

  errors: string;
  isRequesting: boolean;

  constructor(private authService: AuthService, private router: Router, private activatedRoute: ActivatedRoute) { }

  signIn(form: NgForm) {
    this.isRequesting = true;
    this.authService.signIn(form.value.email, form.value.password, form.value.confirmPassword)
      .finally(() => this.isRequesting = false)
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['/log-in']);
          }
        },
        error => this.errors = error);
  }

}

