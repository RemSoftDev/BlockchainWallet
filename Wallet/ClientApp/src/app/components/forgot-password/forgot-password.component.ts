import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../shared/services/auth.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {

  errors: string;
  isRequesting: boolean;

  constructor(private authService: AuthService) { }

  forgotPass(form: NgForm) {
    this.isRequesting = true;
    this.authService.forgotPass(form.value.email)
      .finally(() => this.isRequesting = false)
      .subscribe(
        result => {
          if (result) {
            alert("Email sent");
          }
        },
        error => this.errors = error);
  }

  ngOnInit() {
  }

}
