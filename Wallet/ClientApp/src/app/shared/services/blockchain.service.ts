import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { WalletInfo } from '../models/walletInfo.interface';
import { TransactionsModel } from '../models/transactionsModel.interface';
import { BaseService } from "./base.service";
import { TokenModel } from '../models/tokenModel';


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

  getSmartContractInfo(contractAddress: string) {
    const params = new HttpParams().set('contractAddress', contractAddress);
    return this.http
      .get<TokenModel>(this.hostUrl + this.baseUrl + '/GetSmartContractInfo', { params })
      .catch(this.handleError);
  }

  getSmartContractInfoByName(name: string) {
    const params = new HttpParams().set('contractName', name);
    return this.http
      .get<TokenModel>(this.hostUrl + this.baseUrl + '/GetSmartContractInfoByName', { params })
      .catch(this.handleError);
  }

  getSmartContractTransactions(accountAddress: string) {
    const params = new HttpParams().set('accountAddress', accountAddress);
    return this.http
      .get<TransactionsModel>(this.hostUrl + this.baseUrl + '/GetSmartContractTransactions', { params })
      .catch(this.handleError);
  }

  getSmartContractTransactionsByName(name: string) {
    const params = new HttpParams().set('contractName', name);
    return this.http
      .get<TransactionsModel>(this.hostUrl + this.baseUrl + '/GetSmartContractTransactionsByName', { params })
      .catch(this.handleError);
  }

  getSmartContractTransactionsByNumber(blockNumber, accountAddress: string) {
    const params = new HttpParams().set("blockNumber", blockNumber).set('accountAddress', accountAddress);
    return this.http
      .get<TransactionsModel>(this.hostUrl + this.baseUrl + '/GetSmartContractTransactions', { params })
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
