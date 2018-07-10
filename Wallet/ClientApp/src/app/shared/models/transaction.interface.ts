export interface Transaction {
  date: Date;
  decimalValue: number;
  from: string;
  hash: string;
  isSucceess: boolean;
  to : string;
  contractAddress: string;
}
