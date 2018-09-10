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
using Nethereum.Web3;
using Wallet.BlockchainAPI;
using Wallet.Helpers;
using Wallet.Models;

namespace Wallet.Notifications
{
    public class NotificationService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
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
            _scopeFactory = scopeFactory;
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
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        if (_lastCheckedBlockNumber == 0)
                            _lastCheckedBlockNumber = (int)(await _explorer.GetLastAvailableBlockNumber()).Value;

                        if ((int)(await _explorer.GetLastAvailableBlockNumber()).Value > _lastCheckedBlockNumber)
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

                            Transaction[] transactions = {};
                            if (_userInfo.onlineUsers.Count > 0)
                                transactions = await GetLastBlockTransactions();

                            foreach (var user in _userInfo.onlineUsers)
                            {
                                var data = await dbContext.UserWatchlist
                                    .Where(w => w.UserEmail.Equals(user.Value.UserName,
                                        StringComparison.CurrentCultureIgnoreCase)).Include(w => w.NotificationOptions)
                                    .ToListAsync();

                                var ids = GetIdNotificatedAddresses(transactions.ToList(), data);

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
                    //await _hubContext.Clients.All.SendAsync("Message", e.Message);
                }
            });
        }

        private bool CheckEtherWasSent(List<Transaction> transactions, UserWatchlist watchListLine)
        {
            return (transactions?.Any(t =>
            {
                if ((t.From?.Equals(watchListLine.Address, StringComparison.CurrentCultureIgnoreCase) ?? false) && 
                    (t.Input?.Equals(Constants.Strings.TransactionType.Usual, StringComparison.CurrentCultureIgnoreCase)??false))
                {
                    return true;
                }
                return false;

            }) ?? false);
        }

        private bool CheckAnyTokenWasSent(List<Transaction> transactions, UserWatchlist watchListLine)
        {
            return (transactions?.Any(t =>
            {
                if ( (t.Input?.StartsWith(Constants.Strings.TransactionType.Transfer) ?? false) && (t.From?.Equals(watchListLine.Address, StringComparison.CurrentCultureIgnoreCase) ?? false))
                {
                    return true;
                }
                return false;
            }) ?? false);
        }

        private bool CheckSpecialTokenWasSent(List<Transaction> transactions, UserWatchlist watchListLine)
        {
            return (transactions?.Any(t =>
            {
                if ((t.To?.Equals(watchListLine.NotificationOptions.TokenOrEtherSentName,
                         StringComparison.CurrentCultureIgnoreCase) ?? false) &&
                    (t.From?.Equals(watchListLine.Address, StringComparison.CurrentCultureIgnoreCase) ?? false))
                {
                    return true;
                }
                return false;
            }) ?? false);
        }


        private bool CheckNumberOfEtherWasSent(List<Transaction> transactions, UserWatchlist watchListLine)
        {
            return (transactions?.Any(t =>
            {
                if ((t.From?.Equals(watchListLine.Address, StringComparison.CurrentCultureIgnoreCase) ?? false) &&
                    (t.Input?.Equals(Constants.Strings.TransactionType.Usual, StringComparison.CurrentCultureIgnoreCase) ?? false))
                {
                    if (Web3.Convert.FromWei(t.Value.Value, 18) >= watchListLine.NotificationOptions.NumberOfTokenOrEtherThatWasSentFrom&&
                        Web3.Convert.FromWei(t.Value.Value, 18) <= watchListLine.NotificationOptions.NumberOfTokenOrEtherThatWasSentTo)
                    {
                        return true;
                    }
                    
                }
                return false;

            }) ?? false);
        }


        private bool CheckNumberOfTokenWasSent(List<Transaction> transactions, UserWatchlist watchListLine)
        {
            return (transactions?.Any(t =>
            {
                if ((t.To?.Equals(watchListLine.NotificationOptions.TokenOrEtherSentName,
                         StringComparison.CurrentCultureIgnoreCase) ?? false) &&
                    (t.From?.Equals(watchListLine.Address, StringComparison.CurrentCultureIgnoreCase) ?? false))
                {
                    var value = Web3.Convert.FromWei(InputDecoder.GetTokenCountAndAddressFromInput(t.Input).Value, watchListLine.NotificationOptions.TokenSentDecimalPlaces);
                    if (value >= watchListLine.NotificationOptions.NumberOfTokenOrEtherThatWasSentFrom &&
                        value <= watchListLine.NotificationOptions.NumberOfTokenOrEtherThatWasSentTo)
                    {
                        return true;
                    }

                }
                return false;

            }) ?? false);
        }

        private bool CheckEtherWasReceived(List<Transaction> transactions, UserWatchlist watchListLine)
        {
            return (transactions?.Any(t =>
            {
                if ((t.To?.Equals(watchListLine.Address, StringComparison.CurrentCultureIgnoreCase) ?? false) &&
                    (t.Input?.Equals(Constants.Strings.TransactionType.Usual, StringComparison.CurrentCultureIgnoreCase) ?? false))
                {
                    return true;
                }
                return false;

            }) ?? false);
        }

        private bool CheckTokenWasReceived(List<Transaction> transactions, UserWatchlist watchListLine)
        {
            return (transactions?.Any(t =>
            {
                var receiver = InputDecoder.GetTokenCountAndAddressFromInput(t.Input).To;

                if (receiver.Equals(watchListLine.Address,StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }

                return false;
            }) ?? false);
        }

        private bool CheckNumberEtherWasReceived(List<Transaction> transactions, UserWatchlist watchListLine)
        {
            return (transactions?.Any(t =>
            {
                if ((t.To?.Equals(watchListLine.Address, StringComparison.CurrentCultureIgnoreCase) ?? false) &&
                    (t.Input?.Equals(Constants.Strings.TransactionType.Usual, StringComparison.CurrentCultureIgnoreCase) ?? false))
                {
                    if (Web3.Convert.FromWei(t.Value.Value, 18) == watchListLine.NotificationOptions.NumberOfTokenOrEtherWasReceived)
                    {
                        return true;
                    }
                }
                return false;

            }) ?? false);
        }

        private bool CheckNumberTokenWasReceived(List<Transaction> transactions, UserWatchlist watchListLine)
        {
            return (transactions?.Any(t =>
            {
                var receiver = InputDecoder.GetTokenCountAndAddressFromInput(t.Input);
                var value = Web3.Convert.FromWei(receiver.Value, watchListLine.NotificationOptions.TokenReceivedDecimalPlaces);

                if (receiver.To != null && receiver.To.Equals(watchListLine.Address, StringComparison.CurrentCultureIgnoreCase)&&
                    value == watchListLine.NotificationOptions.NumberOfTokenOrEtherWasReceived)
                {
                    return true;
                }

                return false;
            }) ?? false);
        }

        private bool CheckNumberOfContractTokenWasSent(List<Transaction> transactions, UserWatchlist watchListLine)
        {
            return (transactions?.Any(t =>
            {
                if (t.To !=null && t.To.Equals(watchListLine.Address, StringComparison.CurrentCultureIgnoreCase))
                {
                    var value = Web3.Convert.FromWei(InputDecoder.GetTokenCountAndAddressFromInput(t.Input).Value, watchListLine.TokenDecimalPlaces);

                    if (value == watchListLine.NotificationOptions.NumberOfContractTokenWasSent)
                    {
                        return true;
                    }
                }
                return false;

            }) ?? false);          
        }

        private bool CheckNumberOfContractTokenWasReceivedByAddress(List<Transaction> transactions, UserWatchlist watchListLine)
        {
            return (transactions?.Any(t =>
            {
                if (t.To != null && t.To.Equals(watchListLine.Address, StringComparison.CurrentCultureIgnoreCase))
                {
                    var receiver = InputDecoder.GetTokenCountAndAddressFromInput(t.Input);
                    var value = Web3.Convert.FromWei(receiver.Value, watchListLine.TokenDecimalPlaces);

                    if (value == watchListLine.NotificationOptions.NumberOfTokenWasReceivedByAddress &&
                        receiver.To.Equals(watchListLine.NotificationOptions.AddressThatReceivedNumberOfToken, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }) ?? false);
        }

        public List<int> GetIdNotificatedAddresses(List<Transaction> transactions, List<UserWatchlist> data)
        {
            var result = new List<int>();
            foreach (var watchListLine in data)
            {
                if (!watchListLine.NotificationOptions.IsWithoutNotifications)
                {
                    if (watchListLine.NotificationOptions.WhenAnythingWasSent)
                    {
                        if (CheckEtherWasSent(transactions, watchListLine) || CheckAnyTokenWasSent(transactions, watchListLine))
                        {
                            result.Add(watchListLine.Id);
                            continue;
                        }
                    }
                    if (watchListLine.NotificationOptions.WhenTokenOrEtherIsSent)
                    {
                        if (watchListLine.NotificationOptions.TokenOrEtherSentName.Equals("ETH",StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (CheckEtherWasSent(transactions, watchListLine))
                            {
                                result.Add(watchListLine.Id);
                                continue;
                            }
                        }
                        else
                        {
                            if (CheckSpecialTokenWasSent(transactions, watchListLine))
                            {
                                result.Add(watchListLine.Id);
                                continue;
                            }
                        }
                    }
                    if (watchListLine.NotificationOptions.WhenNumberOfTokenOrEtherWasSent)
                    {
                        if (watchListLine.NotificationOptions.NumberOfTokenOrEtherWasSentName.Equals("ETH", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (CheckNumberOfEtherWasSent(transactions, watchListLine))
                            {
                                result.Add(watchListLine.Id);
                                continue;
                            }
                        }
                        else
                        {
                            if (CheckNumberOfTokenWasSent(transactions, watchListLine))
                            {
                                result.Add(watchListLine.Id);
                                continue;
                            }
                        }
                    }
                    if (watchListLine.NotificationOptions.WhenTokenOrEtherIsReceived)
                    {
                        if (watchListLine.NotificationOptions.TokenOrEtherReceivedName.Equals("ETH", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (CheckEtherWasReceived(transactions, watchListLine))
                            {
                                result.Add(watchListLine.Id);
                                continue;
                            }
                        }
                        else
                        {
                            if (CheckTokenWasReceived(transactions, watchListLine))
                            {
                                result.Add(watchListLine.Id);
                                continue;
                            }
                        }
                    }
                    if (watchListLine.NotificationOptions.WhenNumberOfTokenOrEtherWasReceived)
                    {
                        if (watchListLine.NotificationOptions.TokenOrEtherWasReceivedName.Equals("ETH", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (CheckNumberEtherWasReceived(transactions, watchListLine))
                            {
                                result.Add(watchListLine.Id);
                                continue;
                            }
                        }
                        else
                        {
                            if (CheckNumberTokenWasReceived(transactions, watchListLine))
                            {
                                result.Add(watchListLine.Id);
                                continue;
                            }
                        }
                    }
                    if (watchListLine.NotificationOptions.WhenNumberOfContractTokenWasSent)
                    {
                        if (CheckNumberOfContractTokenWasSent(transactions, watchListLine))
                        {
                            result.Add(watchListLine.Id);
                            continue;
                        }
                    }
                    if (watchListLine.NotificationOptions.WhenNumberOfContractWasReceivedByAddress)
                    {
                        if (CheckNumberOfContractTokenWasReceivedByAddress(transactions, watchListLine))
                        {
                            result.Add(watchListLine.Id);
                            continue;
                        }
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