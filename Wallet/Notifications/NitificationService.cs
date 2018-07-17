using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace Wallet.Notifications
{
    public class NitificationService : IHostedService, IDisposable
    {
        private IHubContext<NotifyHub> _hubContext;
        private IUserInfoInMemory _userInfo;
        private Timer _timer;

        public NitificationService(IHubContext<NotifyHub> hubContext, IUserInfoInMemory userInfo)
        {
            _hubContext = hubContext;
            _userInfo = userInfo;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _hubContext.Clients.Clients(_userInfo.GetUserInfo("")?.ConnectionId)
                .SendAsync("Message", "testTitle", "test payload");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}