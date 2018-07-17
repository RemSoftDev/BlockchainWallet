import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { BlockchainService } from '../../shared/services/blockchain.service';
import { SmartContractInfo } from "../../shared/models/smartContractInfo.interface";
import { NgForm } from '@angular/forms';
import { TransactionsModel } from "../../shared/models/transactionsModel.interface";

@Component({
  selector: 'app-contract-page',
  templateUrl: './contract-page.component.html',
  styleUrls: ['./contract-page.component.css']
})
export class ContractPageComponent implements OnInit, OnDestroy {

  smartContractInfo: SmartContractInfo;
  infoRequesting: boolean = false;
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
  walletsTabIsActive: boolean = false;

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
    this.searchString = this.activatedRoute.snapshot.paramMap.get('searchString');

    this.BCservice.getSmartContractInfo(this.searchString).subscribe(info => {
        this.infoRequesting = true;
        this.smartContractInfo = info;
      },
      error => {
        console.log(error);
        this.errors = true;
      });
  }

  loadTransactions() {
    this.walletsTabIsActive = false;
    this.transactionTabClass = 'active';
    this.walletsTabClass = '';
  }

  loadWallets() {
    this.walletsTabIsActive = true;
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

  sortByDateTime(form: NgForm) {
    console.log(form.value.dateFrom);
    console.log(form.value.timeFrom);
    console.log(form.value.dateTo);
    console.log(form.value.timeTo);
  }

  ngOnInit() {
  }

  ngOnDestroy() {

  }

}
