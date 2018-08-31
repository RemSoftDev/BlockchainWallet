import { Component, OnInit } from '@angular/core';
import { PageDataService } from '../../shared/services/pageData.service';
import { TokenService } from '../../shared/services/adminToken.service';
import { TokenModel } from '../../shared/models/tokenModel';
import { UpdateTokenModel } from '../../shared/models/updateTokenModel';
import { PageData } from '../../shared/models/pageData.interface';
import { BlockchainService } from '../../shared/services/blockchain.service'
import { NgForm } from '@angular/forms';
import { StausSyncTr } from "../../shared/models/StatusSyncTransaction.interface";


@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {

  showAddNewContract: boolean = false;
  contentTabClass: string = 'active';
  contractTabClass: string;
  isContentOpened: boolean = true;
  isContractsOpened: boolean = false;
  isGotContract: boolean = false;
  token: TokenModel;
  isRequested: boolean;
  addressBTC: PageData;
  addressETH: PageData;
  aboutPage: PageData;
  contactPage: PageData;
  tokens: TokenModel[];
  statusSync: StausSyncTr;
  errors: string;




  constructor(private pageDataService: PageDataService, private tokenService: TokenService, private blockChainService: BlockchainService) {}

  ngOnInit() {
    this.loadData();
  }

  postData() {
    let pageData: PageData[] = [];
    pageData.push(this.addressBTC);
    pageData.push(this.addressETH);
    pageData.push(this.aboutPage);
    pageData.push(this.contactPage);
    this.pageDataService.postPageData(pageData).subscribe(data => {
        alert("Updated");
      },
      error => this.errors = error);
  }

  loadData() {
    this.pageDataService.getPageData().subscribe(data => {
        for (let entry of data) {
          if (entry.elementName === "TipsETH") {
            this.addressETH = entry;
          }
          if (entry.elementName === "TipsBTC") {
            this.addressBTC = entry;
          }
          if (entry.elementName === "AboutPage") {
            this.aboutPage = entry;
          }
          if (entry.elementName === "ContactPage") {
            this.contactPage = entry;
          }
        }
        this.isRequested = true;
      },
      error => console.log(error));

    this.blockChainService.getSyncStatus().subscribe(res => {
      this.statusSync = res;
    },
      error => this.errors = error);


  }

  newContract() {
    this.showAddNewContract = !this.showAddNewContract;
  }

  addContract(form: NgForm) {
    let token = new UpdateTokenModel();
    token.address = form.value.address;
    token.decimalplaces = form.value.decimalPlaces;
    token.name = form.value.name;
    token.symbol = form.value.symbol;
    token.webSiteLink = form.value.webSiteLink;
    token.time = new Date(form.value.createdDate).getTime() / 1000;
    this.tokenService.addToken(token).subscribe(data => {
      this.showAddNewContract = false;
      alert("Added");
    },
     error => this.errors = error);
  }

  updateContract(form: NgForm) {  
    this.tokenService.updateContract(this.token).subscribe(data => {
        alert("Updated");
      },
      error => this.errors = error);
  }

  getSmartContract(form: NgForm) {
    this.errors = null;
    this.isGotContract = false;
    this.tokenService.getContractInfo(form.value.contractAddress).subscribe(
      result => {
        this.token = result;
        this.isGotContract = true;
      },
      error => this.errors = error);
  }

  getAllSmartContract(form: NgForm) {
    this.errors = null;    
    this.tokenService.getAllContractInfo().subscribe(res => {
      this.tokens = res;
    },
      error => this.errors = error);
  }

  getSyncStatus(form: NgForm) {
    this.errors = null;
    this.blockChainService.getSyncStatus().subscribe(res => {
      this.statusSync = res;
    },
      error => this.errors = error);
  }

  

  switchToContent() {
    this.isContentOpened = true;
    this.isContractsOpened = false;
    this.contentTabClass = 'active';
    this.contractTabClass = '';
  }

  switchToContracts() {
    this.isContentOpened = false;
    this.isContractsOpened = true;
    this.contentTabClass = '';
    this.contractTabClass = 'active';

    this.tokenService.getAllContractInfo().subscribe(res => {
      this.tokens = res;
    },
      error => this.errors = error);
  }

  saveLatestTransactions() {
    this.blockChainService.saveLatestTransactions().subscribe();
    alert('Started');
  }

}
