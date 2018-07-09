import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BlockchainService } from '../../shared/services/blockchain.service';
import { WalletInfo } from "../../shared/models/walletInfo.interface";
import { Transaction } from "../../shared/models/transaction.interface";


@Component({
  selector: 'app-wallet-page',
  templateUrl: './wallet-page.component.html',
  styleUrls: ['./wallet-page.component.css']
})
export class WalletPageComponent implements OnInit {

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
  transactions: Transaction[];

  constructor(private route: ActivatedRoute, private BCservice: BlockchainService) {}

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

  ngOnInit() {

    this.searchString = this.route.snapshot.paramMap.get('searchString');

    this.BCservice.getTransactions(this.searchString).subscribe(trans => {
        this.transactionRequesting = true;
        this.transactions = trans;
      },
      error => console.log(error));

    this.BCservice.getWalletInfo(this.searchString).subscribe(info => {
        this.infoRequesting = true;
        this.walletInfo = info;
      },
      error => console.log(error));
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
