import { Component, OnInit} from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../shared/services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { CookieService } from 'angular2-cookie/services/cookies.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
})

export class LoginComponent implements OnInit {

  errors: string;
  isRequesting: boolean;
  username: string;
  password: string;
  rememberme: string;

  constructor(private cookieService: CookieService,private authService: AuthService, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    if (this.cookieService.get('remember')) {
      this.username = this.cookieService.get('username');
      this.password = this.cookieService.get('password');
      this.rememberme = this.cookieService.get('remember');
      console.log(this.username);
    }
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
