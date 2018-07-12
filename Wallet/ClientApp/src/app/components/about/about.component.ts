import { Component, OnInit } from '@angular/core';
import { PageDataService } from '../../shared/services/pageData.service';
import { PageData } from '../../shared/models/pageData.interface';

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements OnInit {

  pageData: PageData[];
  aboutPageData: string;

  constructor(private pageDataService: PageDataService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.pageDataService.getPageData().subscribe(data => {
        this.pageData = data;
        for (let entry of this.pageData) {
          if (entry.elementName === "AboutPage") {
            this.aboutPageData = entry.elementData;
          }
        }
      },
      error => console.log(error));
  }

}
