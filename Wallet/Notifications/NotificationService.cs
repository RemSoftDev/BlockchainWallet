using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wallet.BlockchainAPI;
using Wallet.Helpers;
using Wallet.Models;
using Wallet.ViewModels;

namespace Wallet.Notifications
{
    public class NotificationService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory scopeFactory;
        private IBlockchainExplorer _explorer;
        private IHubContext<NotifyHub> _hubContext;
        private IUserInfoInMemory _userInfo;
        private Timer _timer;
        private int _lastCheckedBlockNumber;

        public NotificationService(IHubContext<NotifyHub> hubContext, IUserInfoInMemory userInfo,
            IBlockchainExplorer explorer, IServiceScopeFactory scopeFactory)
        {
            _explorer = explorer;
            _hubContext = hubContext;
            _userInfo = userInfo;
            this.scopeFactory = scopeFactory;
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
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
                        if (_lastCheckedBlockNumber == 0)
                            _lastCheckedBlockNumber = (int)(await _explorer.GetLastAvailableBlockNumber()).Value;

                        if ((int)(await _explorer.GetLastAvailableBlockNumber()).Value > _lastCheckedBlockNumber)
                        {
                            foreach (var user in _userInfo._onlineUsers)
                            {
                                var data = await dbContext.UserWatchlist
                                    .Where(w => w.UserEmail.Equals(user.Value.UserName, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();

                                var result = WatchlistHelper.OrganizeData(data);

                                for (int i = 0; i < result.Count; i++)
                                {
                                    if (_lastCheckedBlockNumber%2==0)
                                    {
                                        result[i].Contract.IsNotificated = true;
                                    }
                                }

                                await _hubContext.Clients.Clients(user.Value.ConnectionId)
                                    .SendAsync("Message", result);
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