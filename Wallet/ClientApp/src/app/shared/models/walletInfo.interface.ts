export interface WalletInfo {
  balance: number;
  tokens: Token[];
}

interface Token {
  address:string;
  symbol:string;      
  decimalPlaces  :number;
  balance : number;
}
