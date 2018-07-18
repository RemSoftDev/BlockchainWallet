import { Component, OnInit, OnDestroy } from '@angular/core';
import { RedirectionService } from '../../shared/services/redirection.service';
import { WatchlistService } from '../../shared/services/watchlist.service';
import { WatchlistModel } from '../../shared/models/watchlist.interface';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';


@Component({
  selector: 'app-watchlist',
  templateUrl: './watchlist.component.html',
  styleUrls: ['./watchlist.component.css']
})



export class WatchlistComponent {
  //users = [
  //  { name: 'qw1' },
  //  { name: 'qw2' }
  //]
  watchlismod: WatchlistModel[];
  constructor(private router: Router, private activatedRoute: ActivatedRoute, private WLservice: WatchlistService) {

    this.WLservice.getWatchlistInfo('0').subscribe(model => {
      this.watchlismod = model
    })


  }


}
