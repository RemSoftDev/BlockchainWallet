import { Component, OnInit } from '@angular/core';
import { PageDataService } from '../../shared/services/pageData.service';
import { PageData } from '../../shared/models/pageData.interface';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {

  pageData: PageData[];
  contactPageData: string;

  constructor(private pageDataService: PageDataService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.pageDataService.getPageData().subscribe(data => {
        this.pageData = data;
        for (let entry of this.pageData) {
          if (entry.elementName === "ContactPage") {
            this.contactPageData = entry.elementData;
          }
        }
      },
      error => console.log(error));
  }

}
