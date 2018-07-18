import { Component, OnInit, OnDestroy } from '@angular/core';
import { RedirectionService } from '../../shared/services/redirection.service';
import { WatchlistService } from '../../shared/services/watchlist.service';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';


@Component({
  selector: 'app-watchlist',
  templateUrl: './watchlist.component.html',
  styleUrls: ['./watchlist.component.css']
})
export class WatchlistComponent implements OnInit, OnDestroy {

  errors;
  watchList;
  requested = false;
  fakeArray:any[];

  constructor(private redirectionService: RedirectionService, private wlService: WatchlistService) {}

  ngOnInit() {
    this.redirectionService.toWatchlistPage();
    this.wlService.getWatchlistInfo(localStorage.getItem('userName')).subscribe(data => {
      this.watchList = data;
      this.requested = true;
      },
      error => this.errors = error);
  }

  ngOnDestroy() {
    this.redirectionService.fromWatchListPage();
  }

}

