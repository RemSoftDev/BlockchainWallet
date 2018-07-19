using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nethereum.RPC.Eth.DTOs;
using Wallet.BlockchainAPI;
using Wallet.Helpers;
using Wallet.Models;

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

                        if ((int)(await _explorer.GetLastAvailableBlockNumber()).Value >= _lastCheckedBlockNumber)
                        {
                            Transaction[] transactions = {};
                            if (_userInfo.onlineUsers.Count > 0)
                                transactions = await GetLastBlockTransactions();

                            foreach (var user in _userInfo.onlineUsers)
                            {
                                var data = await dbContext.UserWatchlist
                                    .Where(w => w.UserEmail.Equals(user.Value.UserName,
                                        StringComparison.CurrentCultureIgnoreCase)).Include(w => w.NotificationOptions)
                                    .ToListAsync();

                                var ids = GetIdNotificatedAddresses(transactions, data);

                                var result = WatchlistHelper.OrganizeData(data);
                                
                                result.ForEach(e =>
                                {
                                    if (ids.Any(i=>i == e.Account.Id))
                                    {
                                        e.Account.IsNotificated = true;
                                    }
                                    if (ids.Any(i => i == e.Contract.Id))
                                    {
                                        e.Contract.IsNotificated = true;
                                    }
                                });

                                await _hubContext.Clients.Clients(user.Value.ConnectionId)
                                    .SendAsync("Message", result);
                            }

                            _lastCheckedBlockNumber++;
                        }
                    }
                }
                catch (Exception e)
                {
                    await _hubContext.Clients.All.SendAsync("Message", "Error");
                }
            });
        }

        public List<int> GetIdNotificatedAddresses(Transaction[] transactions, List<UserWatchlist> data)
        {
            var result = new List<int>();

            foreach (var watchListLine in data)
            {
                if (!watchListLine.NotificationOptions.IsWithoutNotifications)
                {
                    if (watchListLine.NotificationOptions.WhenAnythingWasSent)
                    {
                        if (transactions.ToList().Any(t => t.From.Equals(watchListLine.Address, StringComparison.CurrentCultureIgnoreCase))||
                            transactions.ToList().Any(t => t.Input.StartsWith(Constants.Strings.TransactionType.Transfer)))
                        {
                            result.Add(watchListLine.Id);
                            continue;
                        }
                    }
                    if (watchListLine.NotificationOptions.WhenTokenOrEtherIsSent)
                    {
                        if (transactions.ToList().Any(t=>t.Input.StartsWith(Constants.Strings.TransactionType.Transfer)))
                        {
                            result.Add(watchListLine.Id);
                            continue;                          
                        }
                    }
                    if (watchListLine.NotificationOptions.WhenNumberOfTokenOrEtherWasSent)
                    {

                    }
                    if (watchListLine.NotificationOptions.WhenTokenOrEtherIsReceived)
                    {

                    }
                    if (watchListLine.NotificationOptions.WhenNumberOfTokenOrEtherWasReceived)
                    {

                    }
                    if (watchListLine.NotificationOptions.WhenNumberOfContractTokenWasSent)
                    {

                    }
                    if (watchListLine.NotificationOptions.WhenNumberOfContractWasReceivedByAddress)
                    {

                    }
                }
            }
            return result;
        }

        public async Task<Transaction[]> GetLastBlockTransactions()
        {
            try
            {
                return (await _explorer.GetBlockByNumber(_lastCheckedBlockNumber)).Transactions;

            }
            catch (Exception e)
            {
                return new Transaction[]{};
            }
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