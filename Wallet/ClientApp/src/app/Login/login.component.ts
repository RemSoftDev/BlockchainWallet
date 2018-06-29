import { Component} from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../shared/services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
})

export class LoginComponent {

  errors: string;
  isRequesting: boolean;

  constructor(private authService: AuthService, private router: Router, private activatedRoute: ActivatedRoute) { }

  login(form: NgForm) {
    this.isRequesting = true;
    this.authService.login(form.value.email, form.value.password)
      .finally(() => this.isRequesting = false)
      .subscribe(
      result => {
        if (result) {
          this.router.navigate(['/']);
        }
      },
      error =>this.errors = error);
  }
}
