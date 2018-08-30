import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { BlockchainService } from '../../shared/services/blockchain.service';
import { WatchlistService } from '../../shared/services/watchlist.service';
import { WatchlistModel } from "../../shared/models/watchlistModel";
import { NotificationOptions } from "../../shared/models/watchlistModel";
import { TokenModel } from "../../shared/models/tokenModel";
import { NgForm } from '@angular/forms';
import { TransactionsModel } from "../../shared/models/transactionsModel.interface";
import { TokenHolder } from "../../shared/models/tokenHolder.interface";


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
  tokenHolders: TokenHolder[];
  errors: boolean;
  holdersRequested: boolean;
  dateFrom:Date;
  dateTo: Date;
  skipCount: number = 0;
  currentSortOrder: string = 'QuantityDesc';
  showWarningMessage: boolean= false;

  isSortByDateTime: boolean = false;
  sortByQuantity: boolean = true;
  sortByTokensSent: boolean = true;
  sortByTokensReceived: boolean = true;
  sortByGeneralTxNumber: boolean = true;
  sortBySentTxNumber: boolean = true;
  sortByReceivedTxNumber: boolean = true;

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

  Sort(param: string) {
    this.holdersRequested = false;
    this.skipCount = 0;
    let order = 'Quantity';

    if (param == 'Quantity') {
      if (this.sortByQuantity) {
        order = 'Quantity';
        this.sortByQuantity = false;
      } else {
        order = 'QuantityDesc';
        this.sortByQuantity = true;
      }
    }
    if (param == 'TokensSent') {
      if (this.sortByTokensSent) {
        order = 'TokensSent';
        this.sortByTokensSent = false;
      } else {
        order = 'TokensSentDesc';
        this.sortByTokensSent = true;
      }
    }
    if (param == 'TokensReceived') {
      if (this.sortByTokensReceived) {
        order = 'TokensReceived';
        this.sortByTokensReceived = false;
      } else {
        order = 'TokensReceivedDesc';
        this.sortByTokensReceived = true;
      }
    }
    if (param == 'GeneralTxNumber') {
      if (this.sortByGeneralTxNumber) {
        order = 'GeneralTransactionsNumber';
        this.sortByGeneralTxNumber = false;
      } else {
        order = 'GeneralTransactionsNumberDesc';
        this.sortByGeneralTxNumber = true;
      }
    }
    if (param == 'SentTxNumber') {
      if (this.sortBySentTxNumber) {
        order = 'SentTransactionsNumber';
        this.sortBySentTxNumber = false;
      } else {
        order = 'SentTransactionsNumberDesc';
        this.sortBySentTxNumber = true;
      }
    }
    if (param == 'ReceivedTxNumber') {
      if (this.sortByReceivedTxNumber) {
        order = 'ReceivedTransactionsNumber';
        this.sortByReceivedTxNumber = false;
      } else {
        order = 'ReceivedTransactionsNumberDesc';
        this.sortByReceivedTxNumber = true;
      }
    }
    this.currentSortOrder = order;

    if (this.isSortByDateTime) {
      this.BCservice.getSortedSmartContractHoldersInfoByDate(this.smartContractInfo.id, order, this.dateFrom, this.dateTo).subscribe(info => {
        this.tokenHolders = info;
        this.holdersRequested = true;
      });
    } else {
      this.BCservice.getSortedSmartContractHoldersInfo(this.smartContractInfo.id, order).subscribe(info => {
        this.tokenHolders = info;
        this.holdersRequested = true;
      });
    }
  }

  loadTransaction(skipElementsNumber) {
    this.moreTransactionRequesting = true;
    this.BCservice.getSmartContractTransactionsByNumber(skipElementsNumber, this.searchString).subscribe(model => {
        this.moreTransactionRequesting = false;
        this.transactionsModel.skipElementsNumber = model.skipElementsNumber;
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
    this.getTokenHoldersInfo();
    this.isWalletsOpen = true;
    this.isTransactionsOpen = false;
    this.walletsTabClass = 'active';
    this.transactionTabClass = '';
  }

  getTokenHoldersInfo() {
    this.holdersRequested = false;
    this.BCservice.getSmartContractHoldersInfo(this.smartContractInfo.id).subscribe(info => {
      this.tokenHolders = info;
      this.holdersRequested = true;
    });
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
    this.showWarningMessage = true;
    setTimeout(() => { this.showWarningMessage = false }, 3000);
    this.skipCount = 0;
    this.holdersRequested = false;
    this.isSortByDateTime = true;
    let dateTimeFrom = new Date(form.value.dateFrom +' ' +form.value.timeFrom);
    let dateTimeTo = new Date(form.value.dateTo + ' ' + form.value.timeTo);
    this.dateFrom = dateTimeFrom;
    this.dateTo = dateTimeTo;

    this.BCservice.getSortedSmartContractHoldersInfoByDate(this.smartContractInfo.id, 'QuantityDesc', dateTimeFrom, dateTimeTo).subscribe(info => {
      this.tokenHolders = info;
      this.holdersRequested = true;
    });

  }

  removeDateTimeSorting() {
    this.skipCount = 0;
    this.isSortByDateTime = false;
    this.currentSortOrder = 'QuantityDesc';
    this.sortByQuantity = true;
    this.sortByTokensSent = false;
    this.sortByTokensReceived = false;
    this.sortByGeneralTxNumber = false;
    this.sortBySentTxNumber = false;
    this.sortByReceivedTxNumber = false;
    this.getTokenHoldersInfo();
  }

  loadMoreHoldersInfo() {
    this.holdersRequested = false;

    if (this.isSortByDateTime) {
      this.BCservice
        .loadMoreSortedSmartContractHoldersInfoByDate(this.skipCount + 40,this.smartContractInfo.id, this.currentSortOrder, this.dateFrom, this.dateTo)
        .subscribe(info => {
          this.tokenHolders = this.tokenHolders.concat(info.holdersInfo);
          this.skipCount = info.skipElementsCount;
          this.holdersRequested = true;
        });
    } else {
      this.BCservice
        .loadMoreSortedSmartContractHoldersInfo(this.skipCount + 40, this.smartContractInfo.id, this.currentSortOrder)
        .subscribe(info => {
          this.tokenHolders = this.tokenHolders.concat(info.holdersInfo);
          this.skipCount = info.skipElementsCount;
          this.holdersRequested = true;
        });
    }

  }

  ngOnInit() {
  }

  ngOnDestroy() {

  }

}
