export class WatchlistModel {
  userEmail: string;
  address: string;
  isContract: boolean;
  notificationOptions: NotificationOptions;

  constructor(email: string, address: string, isContract: boolean, notificationOptions: NotificationOptions) {
    this.address = address;
    this.userEmail = email;
    this.isContract = isContract;
    this.notificationOptions = notificationOptions;
  }
}

export class NotificationOptions {
  isWithoutNotifications: boolean;
  whenTokenIsSent: boolean;
  tokenSentName: string;
  whenAnythingWasSent: boolean;
  whenNumberOfTokenWasSent: boolean;
  numberOfTokenThatWasSent: number = 0;
  numberOfTokenWasSentName: string;
  whenTokenIsReceived: boolean;
  tokenReceivedName: string;
  whenNumberOfTokenWasReceived: boolean;
  numberOfTokenWasReceived: number = 0;
  tokenWasReceivedName: string;
  whenNumberOfContractTokenWasSent: boolean;
  numberOfContractTokenWasSent: number = 0;
  whenNumberOfContractWasReceivedByAddress: boolean;
  numberOfTokenWasReceivedByAddress: number = 0;
  addressThatReceivedNumberOfToken: string;
}
