import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { WalletInfo } from '../models/walletInfo.interface';
import { TransactionsModel } from '../models/transactionsModel.interface';
import { BaseService } from "./base.service";

@Injectable()
export class BlockchainService extends BaseService {
  hostUrl: string;
  baseUrl: string = "api/BlockchainData";

  constructor(private http: HttpClient, @Inject('BASE_URL') hostUrl: string) {
    super();
    this.hostUrl = hostUrl;
  }

  checkAddress(address: string) {
    const params = new HttpParams().set('address', address);
    return this.http
      .get(this.hostUrl + this.baseUrl + '/IsContract', { params })
      .catch(this.handleError);
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
      .get<TransactionsModel>(this.hostUrl + this.baseUrl + '/GetTransactions', { params })
      .catch(this.handleError);
  }

  getTransactionsByNumber(blockNumber,accountAddress: string) {
    const params = new HttpParams().set("blockNumber", blockNumber).set('accountAddress', accountAddress);
    return this.http
      .get<TransactionsModel>(this.hostUrl + this.baseUrl + '/GetTransactions', { params })
      .catch(this.handleError);
  }
}
