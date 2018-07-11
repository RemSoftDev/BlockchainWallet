import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/Rx';

@Injectable()
export class RedirectionService  {

  private _redirectionStatusSource = new BehaviorSubject<boolean>(false);
  redirectionhNavStatus$ = this._redirectionStatusSource.asObservable();

  toWatchlistPage() {
    this._redirectionStatusSource.next(true);
  }

  fromWatchListPage() {
    this._redirectionStatusSource.next(false);
  }
}
