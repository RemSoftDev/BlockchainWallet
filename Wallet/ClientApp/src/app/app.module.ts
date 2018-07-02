import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { JwtInterceptor } from './jwt.interceptor';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { FooterMenuComponent } from './footer-menu/footer-menu.component';
import { LoginComponent } from './Login/login.component';
import { SigninComponent } from './sign-in/signin.component';
import { AuthService } from './shared/services/auth.service';
import { BlockchainService } from './shared/services/blockchain.service';
import { WalletPageComponent } from './wallet-page/wallet-page.component';
import { SpinnerComponent } from './spinner/spinner.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    FetchDataComponent,
    FooterMenuComponent,
    LoginComponent,
    SigninComponent,
    WalletPageComponent,
    SpinnerComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'search/:searchString', component: WalletPageComponent},
      { path: 'sign-in', component: SigninComponent },
      { path: 'log-in', component: LoginComponent },
    ])
  ],
  providers: [
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
