import { Component, OnInit} from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../shared/services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { CookieService } from 'ngx-cookie';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {

  errors: string;
  isRequesting: boolean;
  username: string;
  password: string;
  rememberme: string;
  showConfMessage: boolean;

  constructor(private cookieService: CookieService, private authService: AuthService, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    if (this.router.url.startsWith("/api/Account/ConfirmEmail") || this.router.url.startsWith("/Account/ConfirmEmail")) {
      this.confirmEmail();
    }

    if (this.router.url.startsWith("/log-in/ConfirmEmail")) {
      this.showConfirmationMessage();
    }

    if (this.cookieService.get('remember')) {
      this.username = this.cookieService.get('username');
      this.password = this.cookieService.get('password');
      this.rememberme = this.cookieService.get('remember');
    }
  }

  showConfirmationMessage() {
    this.showConfMessage = true;
    setTimeout(() => { this.showConfMessage = false }, 5000);
  }

  confirmEmail() {
    let code: string;
    let userId: string;
    this.activatedRoute.queryParams.subscribe(param => {
      code = param['code'];
      userId = param['userId'];
    });
    this.authService.confirmEmail(userId, code)
      .finally(() => this.isRequesting = false)
      .subscribe(
        result => {
          alert('Email confirmed');
        },
        error => this.errors = error);
  }

  login(form: NgForm) {
    this.isRequesting = true;
    this.authService.login(form.value.email, form.value.password)
      .finally(() => this.isRequesting = false)
      .subscribe(
      result => {
        if (result) {
          this.cookieService.put('username', form.value.email);
          this.cookieService.put('password', form.value.password);
          this.cookieService.put('remember', form.value.rememberme);
          this.router.navigate(['/']);
        }
      },
      error =>this.errors = error);
  }
}
