import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd  } from '@angular/router';
import { BlockchainService } from '../../shared/services/blockchain.service';
import { WalletInfo } from "../../shared/models/walletInfo.interface";
import { TransactionsModel } from "../../shared/models/transactionsModel.interface";


@Component({
  selector: 'app-wallet-page',
  templateUrl: './wallet-page.component.html',
  styleUrls: ['./wallet-page.component.css']
})
export class WalletPageComponent implements OnInit, OnDestroy {

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
  errors;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private BCservice: BlockchainService) {
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
      error => { console.log(error); this.errors = true});

    this.BCservice.getWalletInfo(this.searchString).subscribe(info => {
        this.infoRequesting = true;
        this.walletInfo = info;
      },
      error => { console.log(error); this.errors = true});
  }

  loadTransaction(blockNumber) {
    this.BCservice.getTransactionsByNumber(blockNumber-50, this.searchString).subscribe(model => {
        this.transactionRequesting = true;
        this.transactionsModel.blockNumber = model.blockNumber;
        this.transactionsModel.transactions = this.transactionsModel.transactions.concat(model.transactions);
      },
      error => { console.log(error); this.errors = true });
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
    whenNumberOfTokenSentValueNumber,
    whenNumberOfTokenSentValueToken,
    whenTokenReceivedValue,
    whenNumberOfTokenReceivedValueNumber,
    whenNumberOfTokenReceivedValueToken) {

    console.log(withNotification.checked);
    console.log(whenTokenSent.checked);
    console.log(whenAnythingSent.checked);
    console.log(whenNumberOfTokenSent.checked);
    console.log(whenTokenReceived.checked);
    console.log(whenNumberOfTokenReceived.checked);
    console.log(whenTokenSentValue.value);
    console.log(whenNumberOfTokenSentValueNumber.value);
    console.log(whenNumberOfTokenSentValueToken.value);
    console.log(whenTokenReceivedValue.value);
    console.log(whenNumberOfTokenReceivedValueNumber.value);
    console.log(whenNumberOfTokenReceivedValueToken.value);
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
