using System.Collections.Concurrent;

namespace Wallet.Notifications
{
    public interface IUserInfoInMemory
    {
        bool AddUpdate(string name, string connectionId);
        void Remove(string name);
        ConcurrentDictionary<string, UserInfo> onlineUsers { get; set; }
    }
}