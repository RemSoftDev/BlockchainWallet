import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { WatchlistModel } from '../models/watchlistModel';
import { BaseService } from "./base.service";

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
  })
};

@Injectable()
export class WatchlistService extends BaseService {

  hostUrl: string;
  baseUrl: string = "api/WatchList";

  constructor(private http: HttpClient, @Inject('BASE_URL') hostUrl: string) {
    super();
    this.hostUrl = hostUrl;
  }

  addToWatchList(watchlist: WatchlistModel) {
    return this.http
      .post(
      this.hostUrl + this.baseUrl + '/AddToWatchlist', JSON.stringify(watchlist), httpOptions)
      .map(res => {
        return true;
      })
      .catch(this.handleError);
  }

  getWatchlistInfo(userEmail: string) {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http
      .get(this.hostUrl + this.baseUrl + '/GetUserWatchlist',{ params })      
      .catch(this.handleError);
  }

  getNotificationOptions(address: string) {
    const params = new HttpParams().set('address', address);
    return this.http
      .get(this.hostUrl + this.baseUrl + '/GetNotificationOptions', { params })
      .catch(this.handleError);
  }

}
