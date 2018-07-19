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
  whenTokenOrEtherIsSent: boolean;
  tokenOrEtherSentName: string;
  whenAnythingWasSent: boolean;

  whenNumberOfTokenOrEtherWasSent: boolean;
  numberOfTokenOrEtherThatWasSentFrom: number = 0;
  numberOfTokenOrEtherThatWasSentTo: number = 0;
  numberOfTokenOrEtherWasSentName: string;
  whenTokenOrEtherIsReceived: boolean;
  tokenOrEtherReceivedName: string;
  whenNumberOfTokenOrEtherWasReceived: boolean;
  numberOfTokenOrEtherWasReceived: number = 0;
  tokenOrEtherWasReceivedName: string;

  whenNumberOfContractTokenWasSent: boolean;
  numberOfContractTokenWasSent: number = 0;
  whenNumberOfContractWasReceivedByAddress: boolean;
  numberOfTokenWasReceivedByAddress: number = 0;
  addressThatReceivedNumberOfToken: string;
}
