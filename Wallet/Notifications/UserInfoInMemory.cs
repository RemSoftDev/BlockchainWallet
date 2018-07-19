using System.Collections.Concurrent;
using System.Runtime.InteropServices.ComTypes;

namespace Wallet.Notifications
{
    public class UserInfoInMemory : IUserInfoInMemory
    {
        public ConcurrentDictionary<string, UserInfo> onlineUsers { get; set; } =
            new ConcurrentDictionary<string, UserInfo>();

        public bool AddUpdate(string name, string connectionId)
        {
            var userAlreadyExists = onlineUsers.ContainsKey(name);

            var userInfo = new UserInfo
            {
                UserName = name,
                ConnectionId = connectionId
            };

            onlineUsers.AddOrUpdate(name, userInfo, (key, value) => userInfo);

            return userAlreadyExists;
        }

        public void Remove(string name)
        {
            UserInfo userInfo;
            onlineUsers.TryRemove(name, out userInfo);
        }
    }
}