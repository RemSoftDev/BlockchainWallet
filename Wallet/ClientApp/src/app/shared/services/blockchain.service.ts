import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { WalletInfo } from '../models/walletInfo.interface';
import { TransactionsModel } from '../models/transactionsModel.interface';
import { BaseService } from "./base.service";
import { TokenModel } from '../models/tokenModel';
import { TokenHolder, HolderModel } from '../models/tokenHolder.interface';


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
    const params = new HttpParams().set("skipElementsNumber", blockNumber).set('accountAddress', accountAddress);
    return this.http
      .get<TransactionsModel>(this.hostUrl + this.baseUrl + '/GetSmartContractTransactions', { params })
      .catch(this.handleError);
  }

  getSmartContractHoldersInfo(contractId : string) {
    const params = new HttpParams().set("contractId", contractId);
    return this.http
      .get<TokenHolder>(this.hostUrl + this.baseUrl + '/GetTokenHoldersInfo', { params })
      .catch(this.handleError);
  }

  
  getSortedSmartContractHoldersInfo(contractId: string, sortOrder:string) {
    const params = new HttpParams().set("contractId", contractId).set("sortOrder",sortOrder);
    return this.http
      .get<TokenHolder>(this.hostUrl + this.baseUrl + '/GetTokenHoldersInfo', { params })
      .catch(this.handleError);
  }

  getSortedSmartContractHoldersInfoByDate(contractId: string, sortOrder: string, from: Date, to: Date) {
    let secFrom = from.getTime()/1000;
    let secTo = to.getTime()/1000;
      
    const params = new HttpParams().set("contractId", contractId).set("secondsFrom", secFrom.toString() ).set("secondsTo",secTo.toString())
      .set("sortOrder",sortOrder);
    return this.http
      .get<TokenHolder>(this.hostUrl + this.baseUrl + '/GetTokenHoldersInfoByDateTime', { params })
      .catch(this.handleError);
  }

  loadMoreSortedSmartContractHoldersInfo(skipCount: number, contractId: string, sortOrder: string) {
    const params = new HttpParams().set("skipElementsCount", skipCount.toString()).set("contractId", contractId).set("sortOrder", sortOrder);
    return this.http
      .get<HolderModel>(this.hostUrl + this.baseUrl + '/GetTokenHoldersInfo', { params })
      .catch(this.handleError);
  }

  loadMoreSortedSmartContractHoldersInfoByDate(skipCount: number, contractId: string, sortOrder: string, from: Date, to: Date) {
    let secFrom = from.getTime() / 1000;
    let secTo = to.getTime() / 1000;

    const params = new HttpParams().set("skipElementsCount", skipCount.toString()).set("contractId", contractId).set("secondsFrom", secFrom.toString()).set("secondsTo", secTo.toString())
      .set("sortOrder", sortOrder);
    return this.http
      .get<TokenHolder>(this.hostUrl + this.baseUrl + '/GetTokenHoldersInfoByDateTime', { params })
      .catch(this.handleError);
  }

  getTransactions(accountAddress: string) {
    const params = new HttpParams().set('accountAddress', accountAddress);
    return this.http
      .get<TransactionsModel>(this.hostUrl + this.baseUrl + '/GetTransactions', { params })
      .catch(this.handleError);
  }

  getTransactionsByNumber(blockNumber,accountAddress: string) {
    const params = new HttpParams().set("skipElementsNumber", blockNumber).set('accountAddress', accountAddress);
    return this.http
      .get<TransactionsModel>(this.hostUrl + this.baseUrl + '/GetTransactions', { params })
      .catch(this.handleError);
  }

  saveLatestTransactions() {
    return this.http
      .get(this.hostUrl + this.baseUrl + '/SaveLatestTransactions')
      .catch(this.handleError);
  }

}
