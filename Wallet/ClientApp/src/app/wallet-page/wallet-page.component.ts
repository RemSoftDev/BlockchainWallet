import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BlockchainService } from '../shared/services/blockchain.service';
import { WalletInfo } from "../shared/models/walletInfo.interface";
import { Transaction } from "../shared/models/transaction.interface";


@Component({
  selector: 'app-wallet-page',
  templateUrl: './wallet-page.component.html',
  styleUrls: ['./wallet-page.component.css']
})
export class WalletPageComponent implements OnInit {

  searchString: string;
  infoRequesting: boolean;
  walletInfo: WalletInfo;
  transactions : Transaction[];

  constructor(private route: ActivatedRoute, private BCservice : BlockchainService) { }

  ngOnInit() {
    this.searchString = this.route.snapshot.paramMap.get('searchString');

    this.BCservice.getTransactions(this.searchString).subscribe(trans => {
      this.transactions = trans;
    }, error => console.log(error));

    this.BCservice.getWalletInfo(this.searchString).subscribe(info => {
      this.infoRequesting = true;
      this.walletInfo = info;
    }, error => console.log(error));
  }

}
