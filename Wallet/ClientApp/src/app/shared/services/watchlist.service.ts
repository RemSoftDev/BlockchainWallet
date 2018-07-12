import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Watchlistinfo } from '../models/watchlist.interface';
import { TransactionsModel } from '../models/transactionsModel.interface';
import { BaseService } from "./base.service";

@Injectable()
export class WatchlistService extends BaseService {
  hostUrl: string;
  baseUrl: string = "api/WatchlistData";

  constructor(private http: HttpClient, @Inject('BASE_URL') hostUrl: string) {
    super();
    this.hostUrl = hostUrl;
  }

  getWatchlistInfo(userid: string) {
    return this.http.get(this.hostUrl + 'api/watchlist')
      .map((response: Response) => response.json())
      .catch(this.handleError);  

  }
  
}
