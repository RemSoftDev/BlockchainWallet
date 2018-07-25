import { Component, OnInit } from '@angular/core';
import { PageDataService } from '../../shared/services/pageData.service';
import { TokenService } from '../../shared/services/adminToken.service';
import { TokenModel } from '../../shared/models/tokenModel';
import { PageData } from '../../shared/models/pageData.interface';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {

  contentTabClass: string = 'active';
  contractTabClass: string;
  isContentOpened: boolean = true;
  isContractsOpened: boolean = false;
  isGotContract: boolean = false;
  createdDate:string;
  token: TokenModel;
  isRequested: boolean;
  addressBTC: PageData;
  addressETH: PageData;
  aboutPage: PageData;
  contactPage: PageData;
  errors: string;

  constructor(private pageDataService: PageDataService, private tokenService: TokenService) {}

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
  }

  updateContract(form: NgForm) {
    let dates = this.createdDate.split('-');
    this.errors = null;
    this.token.createdDate = new Date(new Date(+dates[2], +dates[0] - 1, +dates[1] + 1));
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
        let date = new Date(result.createdDate);
        let resDate = (date.getMonth()+1) + '-' + date.getDate() + '-' + date.getFullYear();
        this.createdDate = resDate;
        this.token = result;
        this.isGotContract = true;
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
  }

}
