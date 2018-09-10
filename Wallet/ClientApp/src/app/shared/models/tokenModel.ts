export class TokenModel {
  id: string;
  address: string;
  symbol: string;
  DecimalPlaces: number;
  types: string;
  name: string;
  createdDate: Date;
  webSiteLink: string;
  quantity: number;
  transactionsCount: number;
  walletsCount: number;


  constructor(address: string, symbol: string, decimalPlaces: number, types: string) {
    this.address = address;
    this.symbol = symbol;
    this.DecimalPlaces = decimalPlaces;
    this.types = types;
  }
}
