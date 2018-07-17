namespace Wallet.Notifications
{
    public interface IUserInfoInMemory
    {
        bool AddUpdate(string name, string connectionId);
        void Remove(string name);
        UserInfo GetUserInfo(string username);
    }
}