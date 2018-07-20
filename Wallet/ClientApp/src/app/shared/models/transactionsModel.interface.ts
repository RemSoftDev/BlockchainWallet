export interface TransactionsModel {
  transactions: Transaction[];
  blockNumber: number;
}

interface Transaction {
  hash: string;
  from: string;
  to: string;
  what: string;
  decimalValue: number;
  date: Date;
  isSucceess: boolean;
  contractAddress:string;
}
