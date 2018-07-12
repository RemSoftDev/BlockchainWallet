import { Component, OnInit } from '@angular/core';
import { PageDataService } from '../../shared/services/pageData.service';
import { PageData } from '../../shared/models/pageData.interface';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {

  isRequested: boolean;
  addressBTC: PageData;
  addressETH: PageData;
  aboutPage: PageData;
  contactPage: PageData;
  errors:string;

  constructor(private pageDataService: PageDataService) { }

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
}
