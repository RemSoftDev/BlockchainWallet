export class TokenModel {
  id: string;
  address: string;
  symbol: string;
  decimalplaces: number;
  types: string;
  name: string;
  createdDate: Date;
  webSiteLink: string;
  quantity: number;
  transactionsCount: number;
  walletsCount: number;


  constructor(address: string, symbol: string, decimalplaces: number, types: string) {
    this.address = address;
    this.symbol = symbol;
    this.decimalplaces = decimalplaces;
    this.types = types;
  }
}
