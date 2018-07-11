import { Component, OnInit, OnDestroy} from '@angular/core';
import { RedirectionService } from '../../shared/services/redirection.service';

@Component({
  selector: 'app-watchlist',
  templateUrl: './watchlist.component.html',
  styleUrls: ['./watchlist.component.css']
})
export class WatchlistComponent implements OnInit, OnDestroy {

  constructor(private redirectionService: RedirectionService ) { }

  ngOnInit() {
    this.redirectionService.toWatchlistPage();
  }

  ngOnDestroy() {
    this.redirectionService.fromWatchListPage();
  }

}
