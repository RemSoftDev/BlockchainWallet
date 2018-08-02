export interface HolderModel {
  holdersInfo:TokenHolder;
  skipElementsCount:number;
}

export interface TokenHolder {
  address: string;
  quantity: number;
  tokensSent: number;
  tokensReceived: number;
  generalTransactionsNumber: number;
  sentTransactionsNumber: number;
  receivedTransactionsNumber: number;
}
