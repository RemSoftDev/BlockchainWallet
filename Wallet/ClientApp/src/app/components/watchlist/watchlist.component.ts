import { Component, OnInit, OnDestroy} from '@angular/core';
import { RedirectionService } from '../../shared/services/redirection.service';
import { WatchlistService } from '../../shared/services/watchlist.service';
import { Watchlistinfo } from "../../shared/models/watchlist.interface";
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';

@Component({
  selector: 'app-watchlist',
  templateUrl: './watchlist.component.html',
  styleUrls: ['./watchlist.component.css']
})


//constructor(private redirectionService: RedirectionService) { }
export class WatchlistComponent implements OnInit, OnDestroy {
  watchlistinfo: Watchlistinfo[]; 


  constructor(private router: Router, private activatedRoute: ActivatedRoute, private WLservice: WatchlistService) {
    this.getWInfo()
  }
  getWInfo() {
    this.WLservice.getWatchlistInfo("0").subscribe(
      data => this.watchlistinfo = data
    )
  }

  ngOnInit() {
  }

  ngOnDestroy() {
   
  }





}



  

  

