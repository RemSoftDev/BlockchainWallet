export interface TransactionsModel {
  transactions: Transaction[];
  skipElementsNumber: number;
}

interface Transaction {
  transactionHash: string;
  fromAddress: string;
  toAddress: string;
  what: string;
  decimalValue: number;
  date: Date;
  isSucceess: boolean;
  contractAddress:string;
}
