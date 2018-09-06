import { Component, OnInit, OnDestroy } from '@angular/core';
import { RedirectionService } from '../../shared/services/redirection.service';
import { WatchlistService } from '../../shared/services/watchlist.service';
import { NotificationService } from '../../shared/services/notifications.service';
import { Subscription } from 'rxjs/Subscription';
import { Router} from '@angular/router';


@Component({
  selector: 'app-watchlist',
  templateUrl: './watchlist.component.html',
  styleUrls: ['./watchlist.component.css']
})
export class WatchlistComponent implements OnInit, OnDestroy {

  errors;
  watchList;
  subscription: Subscription;
  requested = false;
  fakeArray:any[];

  constructor(private redirectionService: RedirectionService, private router: Router, private wlService: WatchlistService,
    private notifService: NotificationService) { }

  ngOnInit() {
    this.subscription = this.notifService.receivingNavStatus$.subscribe(() => {
        this.getNitificatedData();
      }
    );
    this.getWatchlist();
    this.redirectionService.toWatchlistPage();

  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.redirectionService.fromWatchListPage();
  }

  goToAddress(address) {
    this.router.navigate(['/search/account', address]);
  }

  goToContract(address) {
    this.router.navigate(['/search/contract', address]);
  }

  getNitificatedData() {
    this.watchList = this.notifService.getData();
  }

  getWatchlist() {
    this.wlService.getWatchlistInfo(localStorage.getItem('userName')).subscribe(data => {
        this.watchList = data;
        this.requested = true;
      },
      error => this.errors = error);
  }

  sortTable(prop: string) {   
    return false; // do not reload
  }

}

