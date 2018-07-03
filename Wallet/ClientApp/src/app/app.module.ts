import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ReCaptchaModule } from 'angular2-recaptcha';
import { CookieService } from 'angular2-cookie/services/cookies.service';

import { JwtInterceptor } from './jwt.interceptor';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { FooterMenuComponent } from './footer-menu/footer-menu.component';
import { LoginComponent } from './Login/login.component';
import { SigninComponent } from './sign-in/signin.component';
import { AuthService } from './shared/services/auth.service';
import { BlockchainService } from './shared/services/blockchain.service';
import { WalletPageComponent } from './wallet-page/wallet-page.component';
import { SpinnerComponent } from './spinner/spinner.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    FooterMenuComponent,
    LoginComponent,
    SigninComponent,
    WalletPageComponent,
    SpinnerComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    ReCaptchaModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'search/:searchString', component: WalletPageComponent},
      { path: 'sign-in', component: SigninComponent },
      { path: 'forgot-password', component: ForgotPasswordComponent },
      { path: 'Account/ResetPassword', component: ResetPasswordComponent },
      { path: 'api/Account/ResetPassword', component: ResetPasswordComponent },
      { path: 'log-in', component: LoginComponent },
    ])
  ],
  providers: [
    CookieService,
    BlockchainService,
    AuthService, {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    }
    ],
  bootstrap: [AppComponent]
})
export class AppModule { }
