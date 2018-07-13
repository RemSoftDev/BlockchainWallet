import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { BlockchainService } from '../../shared/services/blockchain.service';
import { WalletInfo } from "../../shared/models/walletInfo.interface";
import { TransactionsModel } from "../../shared/models/transactionsModel.interface";

@Component({
  selector: 'app-contract-page',
  templateUrl: './contract-page.component.html',
  styleUrls: ['./contract-page.component.css']
})
export class ContractPageComponent implements OnInit, OnDestroy {

  infoRequesting: boolean = true;
  transactionRequesting: boolean = true;
  showNotWind: boolean;
  isWithNotifications: boolean = false;
  isNumberOfTokenSent: boolean = false;
  isNumberOfTokenReceived: boolean = false;
  transactionTabClass: string = 'active';
  walletsTabClass: string;
  navigationSubscription;
  searchString: string;
  errors: boolean;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private BCservice: BlockchainService) {
    this.navigationSubscription = this.router.events.subscribe((e: any) => {
      if (e instanceof NavigationEnd) {
        this.initialise();
      }
    });
  }

  initialise() {
    this.errors = false;

    this.searchString = this.activatedRoute.snapshot.paramMap.get('searchString');

  }

  loadTransactions() {
    this.transactionTabClass = 'active';
    this.walletsTabClass = '';
  }

  loadWallets() {
    this.walletsTabClass = 'active';
    this.transactionTabClass = '';
  }

  setNotification(withNotification,
    whenNumberOfTokenSent,
    whenNumberOfTokenReceived,
    whenNumberOfTokenSentValue,
    whenNumberOfTokenReceivedValue,
    whenNumberOfTokenReceivedAddress) {

    console.log(withNotification.checked);
    console.log(whenNumberOfTokenSent.checked);
    console.log(whenNumberOfTokenReceived.checked);
    console.log(whenNumberOfTokenSentValue.value);
    console.log(whenNumberOfTokenReceivedValue.value);
    console.log(whenNumberOfTokenReceivedAddress.value);
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
    this.isNumberOfTokenSent = false;
    this.isNumberOfTokenReceived = false;
  }

  whenNumberOfTokenReceivedFunc() {
    this.isNumberOfTokenReceived = !this.isNumberOfTokenReceived;
  }

  whenNumberOfTokenSentFunc() {
    this.isNumberOfTokenSent = !this.isNumberOfTokenSent;
  }

  ngOnInit() {
  }

  ngOnDestroy() {

  }

}
