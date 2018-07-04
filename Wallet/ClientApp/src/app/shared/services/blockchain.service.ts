import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { WalletInfo } from '../models/walletInfo.interface';
import { Transaction } from '../models/transaction.interface';
import { BaseService } from "./base.service";

@Injectable()
export class BlockchainService extends BaseService {
  hostUrl: string;
  baseUrl: string = "api/BlockchainData";

  constructor(private http: HttpClient, @Inject('BASE_URL') hostUrl: string) {
    super();
    this.hostUrl = hostUrl;
  }

  getWalletInfo(accountAddress: string) {
    const params = new HttpParams().set('accountAddress', accountAddress);
    return this.http
      .get<WalletInfo>(this.hostUrl + this.baseUrl + '/GetWalletInfo', { params })
      .catch(this.handleError);
  }

  getTransactions(accountAddress: string) {
    const params = new HttpParams().set('accountAddress', accountAddress);
    return this.http
      .get<Transaction[]>(this.hostUrl + this.baseUrl + '/GetTransactions', { params })
      .catch(this.handleError);
  }
}
