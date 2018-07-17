using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Wallet.BlockchainAPI;

namespace Wallet.Notifications
{
    public class NitificationService : IHostedService, IDisposable
    {
        private IBlockchainExplorer _explorer;
        private IHubContext<NotifyHub> _hubContext;
        private IUserInfoInMemory _userInfo;
        private Timer _timer;
        private int _lastCheckedBlockNumber;

        public NitificationService(IHubContext<NotifyHub> hubContext, IUserInfoInMemory userInfo,
            IBlockchainExplorer explorer)
        {
            _explorer = explorer;
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
            Task.Run(async () =>
            {
                try
                {
                    if (_lastCheckedBlockNumber == 0)
                        _lastCheckedBlockNumber = (int)(await _explorer.GetLastAvailableBlockNumber()).Value;

                    if ((int)(await _explorer.GetLastAvailableBlockNumber()).Value > _lastCheckedBlockNumber)
                    {
                        foreach (var user in _userInfo._onlineUsers)
                        {
                            await _hubContext.Clients.Clients(user.Value.ConnectionId)
                                .SendAsync("Message", $"{(int)(await _explorer.GetLastAvailableBlockNumber()).Value} > {_lastCheckedBlockNumber}");
                        }

                        _lastCheckedBlockNumber++;
                    }
                    //else
                    //{
                    //    foreach (var user in _userInfo._onlineUsers)
                    //    {
                    //        await _hubContext.Clients.Clients(user.Value.ConnectionId)
                    //            .SendAsync("Message", $"{(int)(await _explorer.GetLastAvailableBlockNumber()).Value} !!> {_lastCheckedBlockNumber}");
                    //    }
                    //}

                }
                catch (Exception e)
                {
                    await _hubContext.Clients.All.SendAsync("Message", "Error");
                }
            });
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