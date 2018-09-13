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
        private bool _isRunning;

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
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(60));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                if (_isRunning)
                    return;

                _isRunning = true;


                List<ERC20Token> tokens;
                using (var dbContext = new WalletDbContext(DbContextOptionsFactory.DbContextOptions()))
                {
                    tokens = dbContext.Erc20Tokens.ToList();
                }

                foreach (var token in tokens)
                {
                    if (!token.IsSynchronized)
                        continue;

                    var lastBlockNumber = (int) (_explorer.GetLastAvailableBlockNumber().Result.Value);

                    var logs = await _explorer.GetFullEventLogs(token, lastBlockNumber,
                        token.LastSynchronizedBlockNumber + 1);

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

                    using (var dbContext = new WalletDbContext(DbContextOptionsFactory.DbContextOptions()))
                    {
                        foreach (var h in holders)
                        {
                            var holder = dbContext.TokenHolders.FirstOrDefault(e =>
                                e.Address.Equals(h.Address, StringComparison.CurrentCultureIgnoreCase));
                            if (holder != null)
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
                        }

                        token.LastSynchronizedBlockNumber = GetNewLastSearchedBlockNumber(lastBlockNumber,
                            token.LastSynchronizedBlockNumber + 1);

                        dbContext.Erc20Tokens.Update(token);
                        dbContext.SaveChanges();
                    }

                    SaveToDb(logs);

                    _isRunning = false;
                }
            }

            catch (Exception e)
            {
                _isRunning = false;
            }
        }

        private int GetNewLastSearchedBlockNumber(int lastBlockNumber, int blocknum = 1)
        {
            int res = blocknum;
            for (var i = blocknum; i <= lastBlockNumber; i += 100)
            {
                if ((i + 99) > lastBlockNumber)
                    break;
                res = i + 99;
            }

            return res;
        }

        private void SaveToDb<T>(List<T> collectionToInsert) where T : class
        {
            WalletDbContext context = null;
            try
            {
                context = new WalletDbContext(DbContextOptionsFactory.DbContextOptions());
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                int count = 0;
                foreach (var entity in collectionToInsert)
                {
                    ++count;
                    context = AddToContext(context, entity, count, 100, true);
                }

                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        private WalletDbContext AddToContext<T>(WalletDbContext context,
            T entity, int count, int commitCount, bool recreateContext) where T : class
        {
            context.Set<T>().Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new WalletDbContext(DbContextOptionsFactory.DbContextOptions());
                    context.ChangeTracker.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}