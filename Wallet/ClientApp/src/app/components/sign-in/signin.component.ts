import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../shared/services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ReCaptchaComponent } from 'angular2-recaptcha';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
})
export class SigninComponent {

  errors: string;
  isRequesting: boolean;
  validCaptcha: boolean;
  @ViewChild(ReCaptchaComponent) captcha: ReCaptchaComponent;

  constructor(private authService: AuthService, private router: Router, private activatedRoute: ActivatedRoute) { }

  handleCorrectCaptcha(data) {
    if (data === "") {
      this.validCaptcha = false;
    } else {
      this.validCaptcha = true;
    }
  } 

  signIn(form: NgForm) {
    this.isRequesting = true;
    const token = this.captcha.getResponse().toString();
    if (token === '') {
      this.isRequesting = false;
      alert('You cant leave Captcha Code empty');
      return;
    }
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

