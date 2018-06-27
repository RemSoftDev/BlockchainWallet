import { Component } from '@angular/core';
import { AbstractControl } from '@angular/forms';
@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
})
export class SigninComponent {
}


export class PasswordValidation {

  static MatchPassword(AC: AbstractControl) {
    let password = AC.get('password').value; // to get value in input tag
    let confirmPassword = AC.get('confirm_password').value; // to get value in input tag
    if (password != confirmPassword) {
      console.log('false');
      AC.get('confirmPassword').setErrors({ MatchPassword: true })
    } else {
      console.log('true');
      return null
    }
  }
}
