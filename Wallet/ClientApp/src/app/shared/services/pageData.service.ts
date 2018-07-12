import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { PageData } from '../models/pageData.interface';
import { BaseService } from "./base.service";

import 'rxjs/add/operator/map';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
  })
};

@Injectable()
export class PageDataService extends BaseService {

  hostUrl: string;
  baseUrl: string = "api/PageData";

  constructor(private http: HttpClient, @Inject('BASE_URL') hostUrl: string) {
    super();
    this.hostUrl = hostUrl;
  }
  
  getPageData() {
    return this.http
      .get<PageData[]>(this.hostUrl + this.baseUrl + '/GetPageElements')
      .catch(this.handleError);
  }

  postPageData(pageData: PageData[]) {
    return this.http
      .post(
        this.hostUrl + this.baseUrl + '/SetPageElements',
        JSON.stringify(pageData),
        httpOptions)
      .map(res => {
        return true;
      })
      .catch(this.handleError);
  }
}
