using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Wallet.Notifications
{
    public class NotifyHub: Hub
    {
        private UserInfoInMemory _userInfoInMemory;

        public NotifyHub(UserInfoInMemory userInfoInMemory)
        {
            _userInfoInMemory = userInfoInMemory;
        }

        public async Task Join(string userName)
        {
            _userInfoInMemory.AddUpdate(userName, Context.ConnectionId);
            await Clients.Client(Context.ConnectionId).SendAsync("Joined", userName);
        }

        public async Task Leave(string userName)
        {
            _userInfoInMemory.Remove(userName);
            await Clients.Client(Context.ConnectionId).SendAsync("Left", $"User - {userName} removed");
        }
    }
}