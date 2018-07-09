export class WithoutNotificationsModel {
  address: string;
  isNotifications: boolean;

  WithoutNotifications(address) {
    this.address = address;
    this.isNotifications = false;
  }
}
