import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { BlockchainService } from '../../shared/services/blockchain.service';
import { WalletInfo } from "../../shared/models/walletInfo.interface";
import { TransactionsModel } from "../../shared/models/transactionsModel.interface";
import { WatchlistService } from '../../shared/services/watchlist.service';
import { NotificationOptions } from "../../shared/models/watchlistModel";
import { WatchlistModel } from "../../shared/models/watchlistModel";

@Component({
  selector: 'app-account-page',
  templateUrl: './account-page.component.html',
  styleUrls: ['./account-page.component.css']
})
export class AccountPageComponent implements OnInit, OnDestroy {
  showNotWind: boolean;
  notificationOptions: NotificationOptions;
  searchString: string;
  infoRequesting: boolean;
  transactionRequesting: boolean;
  walletInfo: WalletInfo;
  transactionsModel: TransactionsModel;
  navigationSubscription;
  moreTransactionRequesting: boolean;
  errors;

  constructor(private router: Router, private activatedRoute: ActivatedRoute,
    private BCservice: BlockchainService, private watchlistService: WatchlistService)
  {
    this.navigationSubscription = this.router.events.subscribe((e: any) => {
      if (e instanceof NavigationEnd) {
        this.initialise();
      }
    });
  }

  initialise() {
    this.infoRequesting = false;
    this.errors = false;
    this.transactionRequesting = false;

    this.searchString = this.activatedRoute.snapshot.paramMap.get('searchString');

    this.BCservice.getTransactions(this.searchString).subscribe(model => {
        this.transactionRequesting = true;
        this.transactionsModel = model;
      },
      error => {
        console.log(error);
        this.errors = true;
      });

    this.BCservice.getWalletInfo(this.searchString).subscribe(info => {
        this.infoRequesting = true;
        this.walletInfo = info;
      },
      error => {
        console.log(error);
        this.errors = true;
      });

    this.requestNotificationOptions();

  }

  requestNotificationOptions() {
    this.watchlistService.getNotificationOptions(this.searchString).subscribe(options => {
        this.notificationOptions = options;
      },
      error => {
        this.notificationOptions = new NotificationOptions();
      });
  }

  loadTransaction(skipElementsNumber) {
    this.moreTransactionRequesting = true;
    this.BCservice.getTransactionsByNumber(skipElementsNumber, this.searchString).subscribe(model => {
        this.moreTransactionRequesting = false;
        this.transactionsModel.skipElementsNumber = model.skipElementsNumber;
        this.transactionsModel.transactions = this.transactionsModel.transactions.concat(model.transactions);
      },
      error => {
        console.log(error);
        this.errors = true;
      });
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  setNotification() {

    let model = new WatchlistModel(localStorage.getItem('userName'), this.searchString, false, this.notificationOptions, 18);

    this.watchlistService.addToWatchList(model).subscribe(data => {
        this.requestNotificationOptions();
        this.closeNotificationWindow();
        alert("Submited");
      },
      error => console.log(error));
  }

  showNotificationWindow() {
    if (localStorage.getItem('access_token')) {
      this.showNotWind = true;
    } else {
      alert("Please, log in");
    }
  }

  closeNotificationWindow() {
    this.showNotWind = false;
  }
}
