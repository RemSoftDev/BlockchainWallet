using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Web3;
using Wallet.BlockchainAPI;
using Wallet.Helpers;
using Wallet.Models;

namespace Wallet.Services
{
    public class BlockchainDataUpdateService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IBlockchainExplorer _explorer;
        private Timer _timer;

        public BlockchainDataUpdateService(IBlockchainExplorer explorer, IServiceScopeFactory scopeFactory)
        {
            _explorer = explorer;
            _scopeFactory = scopeFactory;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //_timer = new Timer(DoWork, null, TimeSpan.Zero,
            //    TimeSpan.FromMinutes(30));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

                foreach (var token in dbContext.Erc20Tokens.ToList())
                {
                    var events = dbContext.CustomEventLogs.Where(l => l.ERC20TokenId == token.Id);
                    if (!events.Any())
                        continue;
                    var lastSearchedBlockNumber = events.Max(l => l.BlockNumber);
                    

                    var logs = await _explorer.GetFullEventLogs(token, lastSearchedBlockNumber);
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

                    holders.ForEach(h =>
                    {
                        var holder = dbContext.TokenHolders.FirstOrDefault(e =>
                            e.Address.Equals(h.Address, StringComparison.CurrentCultureIgnoreCase));
                        if (holder !=null)
                        {
                            holder.Quantity = h.Quantity;
                            holder.GeneralTransactionsNumber += h.GeneralTransactionsNumber;
                            holder.SentTransactionsNumber += h.SentTransactionsNumber;
                            holder.ReceivedTransactionsNumber += h.ReceivedTransactionsNumber;
                            holder.TokensSent += h.TokensSent;
                            holder.TokensReceived += h.TokensReceived;
                            dbContext.TokenHolders.Update(holder);
                        }
                        else
                        {
                            dbContext.TokenHolders.Add(h);
                        }

                    });
                    dbContext.CustomEventLogs.AddRange(logs);
                    dbContext.SaveChanges();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
