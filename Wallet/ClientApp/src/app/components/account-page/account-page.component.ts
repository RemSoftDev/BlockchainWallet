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
  isTokenSent: boolean = false;
  isNumberOfTokenSent: boolean = false;
  isTokenReceived: boolean = false;
  isNumberOfTokenReceived: boolean = false;
  isWithNotifications: boolean = false;
  showNotWind: boolean;
  searchString: string;
  infoRequesting: boolean;
  transactionRequesting: boolean;
  walletInfo: WalletInfo;
  transactionsModel: TransactionsModel;
  navigationSubscription;
  moreTransactionRequesting: boolean;
  errors;

  constructor(private router: Router, private activatedRoute: ActivatedRoute,
    private BCservice: BlockchainService, private watchlistService: WatchlistService) {
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

  setNotification(withNotification,
    whenTokenSent,
    whenAnythingSent,
    whenNumberOfTokenSent,
    whenTokenReceived,
    whenNumberOfTokenReceived,
    whenTokenSentValue,
    whenNumberOfTokenSentValueNumberFrom,
    whenNumberOfTokenSentValueNumberTo,
    whenNumberOfTokenSentValueToken,
    whenTokenReceivedValue,
    whenNumberOfTokenReceivedValueNumber,
    whenNumberOfTokenReceivedValueToken) {

    let notifOptions = new NotificationOptions();

    if (withNotification.checked) {
      notifOptions.isWithoutNotifications = true;
    } else {
      notifOptions.whenTokenOrEtherIsSent = whenTokenSent.checked;
      notifOptions.tokenOrEtherSentName = whenTokenSentValue.value;
      notifOptions.whenAnythingWasSent = whenAnythingSent.checked;
      notifOptions.whenNumberOfTokenOrEtherWasSent = whenNumberOfTokenSent.checked;
      if (whenNumberOfTokenSentValueNumberFrom.value)
        notifOptions.numberOfTokenOrEtherThatWasSentFrom = whenNumberOfTokenSentValueNumberFrom.value;
      if (whenNumberOfTokenSentValueNumberTo.value)
        notifOptions.numberOfTokenOrEtherThatWasSentTo = whenNumberOfTokenSentValueNumberTo.value;
      notifOptions.numberOfTokenOrEtherWasSentName = whenNumberOfTokenSentValueToken.value;
      notifOptions.whenTokenOrEtherIsReceived = whenTokenReceived.checked;
      notifOptions.tokenOrEtherReceivedName = whenTokenReceivedValue.value;
      notifOptions.whenNumberOfTokenOrEtherWasReceived = whenNumberOfTokenReceived.checked;
      if (whenNumberOfTokenReceivedValueNumber.value)
        notifOptions.numberOfTokenOrEtherWasReceived = whenNumberOfTokenReceivedValueNumber.value;
     
      notifOptions.tokenOrEtherWasReceivedName = whenNumberOfTokenReceivedValueToken.value;
    }

    let model = new WatchlistModel(localStorage.getItem('userName'), this.searchString, false, notifOptions);

    this.watchlistService.addToWatchList(model).subscribe(data => {
        this.closeNotificationWindow();
        alert("Added");
      },
      error => this.errors = error);
  }

  whenNumberOfTokenReceivedFunc() {
    this.isNumberOfTokenReceived = !this.isNumberOfTokenReceived;
  }

  whenTokenReceivedFunc() {
    this.isTokenReceived = !this.isTokenReceived;
  }

  whenNumberOfTokenSentFunc() {
    this.isNumberOfTokenSent = !this.isNumberOfTokenSent;
  }

  whenTokenSentFunc() {
    this.isTokenSent = !this.isTokenSent;
  }

  withNotifications() {
    this.isWithNotifications = this.isWithNotifications == false;
  }

  showNotificationWindow() {
    if (localStorage.getItem('access_token')) {
      this.showNotWind = true;
    } else {
      alert("Please, log in");
    }
  }

  closeNotificationWindow() {
    if (this.isWithNotifications) {
      this.isWithNotifications = false;
    }
    this.showNotWind = false;
    this.isTokenSent = false;
    this.isNumberOfTokenSent = false;
    this.isTokenReceived = false;
    this.isNumberOfTokenReceived = false;
  }
}
