import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../shared/services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {

  errors: string;
  isRequesting: boolean;
  code: string;

  constructor(private authService: AuthService, private router: Router, private activatedRoute: ActivatedRoute) { }

  resetPass(form: NgForm) {
    this.isRequesting = true;
    this.authService.resetPass(form.value.email, form.value.password, this.code )
      .finally(() => this.isRequesting = false)
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['/log-in']);
          }
        },
        error => this.errors = error);
  }

  ngOnInit() {
    this.activatedRoute.queryParams.subscribe(param => {
      this.code = param['code'];
    });
  }

}
