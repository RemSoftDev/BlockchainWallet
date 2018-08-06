import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { TokenModel } from '../models/tokenModel';
import { UpdateTokenModel } from '../models/updateTokenModel';
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

  updateContract(token: TokenModel) {
    return this.http
      .post(
      this.hostUrl + this.baseUrl + '/UpdateSmartContract', JSON.stringify(token), httpOptions)
      .map(res => {
        return true;
      })
      .catch(this.handleError);
  }

  getContractInfo(contractAddress: string) {
    const params = new HttpParams().set('contractAddress', contractAddress);
    return this.http
      .get <TokenModel>(this.hostUrl + this.baseUrl + '/GetSmartContract', { params })
      .catch(this.handleError);
  }

  getAllContractInfo() {    
    return this.http
      .get<TokenModel[]>(this.hostUrl + this.baseUrl + '/GetAllContract')
      .catch(this.handleError);
  }

  addToken(token: UpdateTokenModel) {
    return this.http
      .post(
        this.hostUrl + this.baseUrl + '/AddSmartContract', JSON.stringify(token), httpOptions)
      .map(res => {
        return true;
      })
      .catch(this.handleError);
  }

}
