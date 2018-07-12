import { Component, OnInit } from '@angular/core';
import { PageDataService } from '../../shared/services/pageData.service';
import { PageData } from '../../shared/models/pageData.interface';

@Component({
  selector: 'app-footer-menu',
  templateUrl: './footer-menu.component.html',
  styleUrls: ['./footer-menu.component.css']
})
export class FooterMenuComponent implements OnInit {
  pageData: PageData[];
  ethAddress: string;
  btcAddress: string;

  constructor(private pageDataService: PageDataService) {}

  ngOnInit() {
    this.loadData();
  }

  loadData(){
    this.pageDataService.getPageData().subscribe(data => {
      this.pageData = data;
      for (let entry of this.pageData) {
        if (entry.elementName === "TipsETH") {
          this.ethAddress = entry.elementData;
        }
        if (entry.elementName === "TipsBTC") {
          this.btcAddress = entry.elementData;
        }
      }
      },
      error => console.log(error));
  }
}
