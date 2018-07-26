import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { BlockchainService } from '../../shared/services/blockchain.service';
import { WatchlistService } from '../../shared/services/watchlist.service';
import { WatchlistModel } from "../../shared/models/watchlistModel";
import { NotificationOptions } from "../../shared/models/watchlistModel";
import { TokenModel } from "../../shared/models/tokenModel";
import { NgForm } from '@angular/forms';
import { TransactionsModel } from "../../shared/models/transactionsModel.interface";

@Component({
  selector: 'app-contract-page',
  templateUrl: './contract-page.component.html',
  styleUrls: ['./contract-page.component.css']
})
export class ContractPageComponent implements OnInit, OnDestroy {

  smartContractInfo: TokenModel;
  isTransactionsOpen: boolean = true;
  isWalletsOpen: boolean = false;
  transactionsModel: TransactionsModel;
  infoRequesting: boolean = false;
  moreTransactionRequesting:boolean = false;
  transactionRequesting: boolean = false;
  showNotWind: boolean;
  isWithNotifications: boolean = false;
  isNumberOfTokenSent: boolean = false;
  isNumberOfTokenReceived: boolean = false;
  transactionTabClass: string = 'active';
  walletsTabClass: string;
  navigationSubscription;
  searchString: string;
  errors: boolean;

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
    this.transactionRequesting = false;
    this.errors = false;
    this.searchString = this.activatedRoute.snapshot.paramMap.get('searchString');

    if (!this.searchString.startsWith('0x')) {
      this.BCservice.getSmartContractInfoByName(this.searchString).subscribe(info => {
          this.infoRequesting = true;
          this.smartContractInfo = info;
          this.searchString = info.address;
        },
        error => {
          console.log(error);
          this.errors = true;
        });


      this.BCservice.getSmartContractTransactionsByName(this.searchString).subscribe(transact => {
          this.transactionRequesting = true;
          this.transactionsModel = transact;
        },
        error => {
          console.log(error);
          this.errors = true;
        });

    } else {
      this.BCservice.getSmartContractInfo(this.searchString).subscribe(info => {
          this.infoRequesting = true;
          this.smartContractInfo = info;
        },
        error => {
          console.log(error);
          this.errors = true;
        });

      this.BCservice.getSmartContractTransactions(this.searchString).subscribe(transact => {
          this.transactionRequesting = true;
          this.transactionsModel = transact;
        },
        error => {
          console.log(error);
          this.errors = true;
        });
    }
  }

  loadTransaction(blockNumber) {
    this.moreTransactionRequesting = true;
    this.BCservice.getSmartContractTransactionsByNumber(blockNumber - 100, this.searchString).subscribe(model => {
        this.moreTransactionRequesting = false;
        this.transactionsModel.blockNumber = model.blockNumber;
        this.transactionsModel.transactions = this.transactionsModel.transactions.concat(model.transactions);
      },
      error => {
        console.log(error);
        this.errors = true;
      });
  }

  SwitchToTransactions() {
    this.isWalletsOpen = false;
    this.isTransactionsOpen = true;
    this.transactionTabClass = 'active';
    this.walletsTabClass = '';
  }

  SwitchToWallets() {
    this.isWalletsOpen = true;
    this.isTransactionsOpen = false;
    this.walletsTabClass = 'active';
    this.transactionTabClass = '';
  }

  setNotification(withNotification,
    whenNumberOfTokenSent,
    whenNumberOfTokenReceived,
    whenNumberOfTokenSentValue,
    whenNumberOfTokenReceivedValue,
    whenNumberOfTokenReceivedAddress) {

    let notifOptions = new NotificationOptions();

    if (withNotification.checked) {
      notifOptions.isWithoutNotifications = true;
    } else {
      notifOptions.whenNumberOfContractTokenWasSent = true;
      if (whenNumberOfTokenSentValue.value)
        notifOptions.numberOfContractTokenWasSent = whenNumberOfTokenSentValue.value;
      notifOptions.whenNumberOfContractWasReceivedByAddress = whenNumberOfTokenReceived.checked;
      if (whenNumberOfTokenReceivedValue.value)
        notifOptions.numberOfTokenWasReceivedByAddress = whenNumberOfTokenReceivedValue.value;
      notifOptions.addressThatReceivedNumberOfToken = whenNumberOfTokenReceivedAddress.value;
    }

    let model = new WatchlistModel(localStorage.getItem('userName'),this.searchString, true,notifOptions);
 
    this.watchlistService.addToWatchList(model).subscribe(data => {
      this.closeNotificationWindow();
      alert("Added");
    },
    error => this.errors = error);

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
