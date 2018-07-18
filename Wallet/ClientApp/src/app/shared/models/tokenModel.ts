export class TokenModel {
  address: string;
  symbol: string;
  decimalplaces: number;
  types: string;


  constructor(address: string, symbol: string, decimalplaces: number, types: string) {
    this.address = address;
    this.symbol = symbol;
    this.decimalplaces = decimalplaces;
    this.types = types;
  }
}
