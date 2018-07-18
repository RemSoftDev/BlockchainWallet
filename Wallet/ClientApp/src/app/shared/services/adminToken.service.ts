import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { TokenModel } from '../models/tokenModel';
import { BaseService } from "./base.service";

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
  })
};

@Injectable()
export class TokenService extends BaseService {

  hostUrl: string;
  baseUrl: string = "api/TokenData";

  constructor(private http: HttpClient, @Inject('BASE_URL') hostUrl: string) {
    super();
    this.hostUrl = hostUrl;
  }

  addToTokenList(token: TokenModel) {
    return this.http
      .post(
      this.hostUrl + this.baseUrl + '/AddToTokenlist', JSON.stringify(token), httpOptions)
      .map(res => {
        return true;
      })
      .catch(this.handleError);
  }

  getWatchlistInfo(userEmail: string) {
    //const params = new HttpParams().set('userEmail', userEmail);
    return this.http
      .get(this.hostUrl + this.baseUrl + '/GetTokenlist')
      .catch(this.handleError);
  }

}
