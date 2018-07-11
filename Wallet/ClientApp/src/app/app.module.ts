import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ReCaptchaModule } from 'angular2-recaptcha';
import { CookieModule } from 'ngx-cookie';

import { JwtInterceptor } from './jwt.interceptor';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';
import { FooterMenuComponent } from './components/footer-menu/footer-menu.component';
import { LoginComponent } from './components/Login/login.component';
import { SigninComponent } from './components/sign-in/signin.component';
import { AuthService } from './shared/services/auth.service';
import { RedirectionService } from './shared/services/redirection.service';
import { BlockchainService } from './shared/services/blockchain.service';
import { WalletPageComponent } from './components/wallet-page/wallet-page.component';
import { SpinnerComponent } from './components/spinner/spinner.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { AboutComponent } from './components/about/about.component';
import { ContactComponent } from './components/contact/contact.component';
import { WatchlistComponent } from './components/watchlist/watchlist.component';

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
    ResetPasswordComponent,
    AboutComponent,
    ContactComponent,
    WatchlistComponent
  ],
  imports: [
    CookieModule.forRoot(),
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    ReCaptchaModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'search/:searchString', component: WalletPageComponent, runGuardsAndResolvers: 'always'},
      { path: 'sign-in', component: SigninComponent },
      { path: 'forgot-password', component: ForgotPasswordComponent },
      { path: 'Account/ResetPassword', component: ResetPasswordComponent },
      { path: 'api/Account/ResetPassword', component: ResetPasswordComponent },
      { path: 'log-in', component: LoginComponent },
      { path: 'about', component: AboutComponent },
      { path: 'contact', component: ContactComponent },
      { path: 'watchlist', component: WatchlistComponent }
    ],
      { onSameUrlNavigation: 'reload' })
  ],
  providers: [
    BlockchainService,
    RedirectionService,
    AuthService, {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    }
    ],
  bootstrap: [AppComponent]
})
export class AppModule { }
