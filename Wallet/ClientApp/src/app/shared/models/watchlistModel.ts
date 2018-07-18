export class WatchlistModel {
  userEmail: string;
  address: string;
  isContract: boolean;

  constructor(email: string, address: string, isContract: boolean) {
    this.address = address;
    this.userEmail = email;
    this.isContract = isContract;
  }
}
