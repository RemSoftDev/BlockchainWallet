import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { WatchlistModel } from '../models/watchlist.interface';
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
    const params = new HttpParams().set('userid', userid);
    return this.http
      .get<WatchlistModel>(this.hostUrl + this.baseUrl + '/watchlist')      
      .catch(this.handleError);
  }

}
