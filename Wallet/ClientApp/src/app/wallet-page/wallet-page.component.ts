import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BlockchainService } from '../shared/services/blockchain.service';
import { WalletInfo } from "../shared/models/walletInfo.interface";

@Component({
  selector: 'app-wallet-page',
  templateUrl: './wallet-page.component.html',
  styleUrls: ['./wallet-page.component.css']
})
export class WalletPageComponent implements OnInit {

  searchString: string;
  infoRequesting: boolean;
  walletInfo : WalletInfo;

  constructor(private route: ActivatedRoute, private BCservice : BlockchainService) { }

  ngOnInit() {
    this.searchString = this.route.snapshot.paramMap.get('searchString');
    this.BCservice.getWalletInfo(this.searchString).subscribe(info => {
      this.infoRequesting = true;
      this.walletInfo = info;
    }, error => console.log(error));
  }

}
