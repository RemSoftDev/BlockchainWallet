using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nethereum.Web3;
using Wallet.BlockchainAPI;
using Wallet.Helpers;
using Wallet.Models;

namespace Wallet.Services
{
    public class EventLogsService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IBlockchainExplorer _explorer;
        private Timer _timer;
        private bool isRunning;


        public EventLogsService(IBlockchainExplorer explorer, IServiceScopeFactory scopeFactory)
        {
            _explorer = explorer;

            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                if (isRunning)
                    return;

                isRunning = true;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

                    var tokens = dbContext.Erc20Tokens.ToList();

                    foreach (var token in tokens)
                    {
                        if (token.IsSynchronized)
                            continue;

                        var lastBlockNumber = (int)(_explorer.GetLastAvailableBlockNumber().Result.Value);

                        var logs = _explorer.GetFullEventLogs(token, lastBlockNumber).Result;

                        var holders = EventLogsExplorer.GetInfoFromLogs(logs);

                        for (int i = 0; i < holders.Count; i++)
                        {
                            try
                            {
                                var balance = _explorer.GetTokenHolderBalance(holders[i].Address, token.Address).Result;
                                holders[i].Quantity = Web3.Convert.FromWei(balance, token.DecimalPlaces);
                                holders[i].ERC20TokenId = token.Id;
                            }
                            catch (Exception e)
                            {
                                i--;
                            }
                        }

                        token.LastSynchronizedBlockNumber = lastBlockNumber;
                        token.IsSynchronized = true;
                        var tempLogs = new List<CustomEventLog>();
                        var tempHolders = new List<TokenHolder>();

                        foreach (var customEventLog in logs)
                        {
                            tempLogs.Add(customEventLog);
                            if (tempLogs.Count == 100)
                            {
                                dbContext.CustomEventLogs.AddRange(tempLogs);
                                dbContext.SaveChanges();
                                tempLogs.Clear();
                            }
                        }

                        dbContext.CustomEventLogs.AddRange(tempLogs);
                        dbContext.SaveChanges();

                        foreach (var tokenHolder in holders)
                        {
                            tempHolders.Add(tokenHolder);
                            if (tempHolders.Count==100)
                            {
                                dbContext.TokenHolders.AddRange(tempHolders);
                                dbContext.SaveChanges();
                                tempHolders.Clear();
                            }
                        }
                        dbContext.TokenHolders.AddRange(tempHolders);
                        dbContext.SaveChanges();

                        dbContext.Erc20Tokens.Update(token);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                isRunning = false;

            }
            isRunning = false;
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