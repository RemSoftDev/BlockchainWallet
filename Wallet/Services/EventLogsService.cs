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

                List<ERC20Token> tokens;
                using (var dbContext = new WalletDbContext(DbContextOptionsFactory.DbContextOptions()))
                {
                    tokens = dbContext.Erc20Tokens.ToList();
                }

                foreach (var token in tokens)
                {
                    if (token.IsSynchronized)
                        continue;

                    var lastBlockNumber = (int) (_explorer.GetLastAvailableBlockNumber().Result.Value);

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

                    SaveToDb(logs);
                    SaveToDb(holders);

                    using (var dbContext = new WalletDbContext(DbContextOptionsFactory.DbContextOptions()))
                    {
                        token.LastSynchronizedBlockNumber = lastBlockNumber;
                        token.IsSynchronized = true;
                        dbContext.Erc20Tokens.Update(token);
                        dbContext.SaveChanges();
                    }
                }
                isRunning = false;
            }
            catch (Exception e)
            {
                isRunning = false;
            }
        }

        private void SaveToDb<T>(List<T> collectionToInsert) where T:class
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
            T entity, int count, int commitCount, bool recreateContext) where T:class
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

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}