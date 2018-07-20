import { Component, OnInit, OnDestroy } from '@angular/core';
import { RedirectionService } from '../../shared/services/redirection.service';
import { WatchlistService } from '../../shared/services/watchlist.service';
import { NotificationsService } from '../../shared/services/notifications.service';
import { Subscription } from 'rxjs/Subscription';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';


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

  constructor(private redirectionService: RedirectionService, private wlService: WatchlistService,
    private notifService: NotificationsService) { }

  ngOnInit() {
    this.subscription = this.notifService.receivingNavStatus$.subscribe(() => {
        this.getNitificatedData();
      }
    );
    this.getWatchlist();
    this.notifService.subscribuToNotifications();
    this.redirectionService.toWatchlistPage();

  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.notifService.unSubscribuFromNotifications();
    this.redirectionService.fromWatchListPage();
  }

  playSound() {
    let audio = new Audio();
    audio.src = "../../assets/notification.mp3";
    audio.load();
    audio.play();
  }

  getNitificatedData() {
    console.log(this.notifService.getData());
    this.watchList = this.notifService.getData();
    if (this.shouldMakeSound())
      this.playSound();
  }

  shouldMakeSound() {
    if (this.watchList) {
      for (let entry of this.watchList) {
        if (entry.account.isNotificated || entry.contract.isNotificated) {
          return true;
        }
      }
      return false;
    }
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

