using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wallet.BlockchainAPI;
using Wallet.Models;

namespace Wallet.Services
{
    public class TransactionService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IBlockchainExplorer _explorer;
        private Timer _timer;
        private Timer _deleteTimer;
        private int _lastCheckedBlockNumber;
        private int _lastBlockNumber;
        private bool isRunning;

        public TransactionService(IBlockchainExplorer explorer, IServiceScopeFactory scopeFactory)
        {
            _explorer = explorer;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            _deleteTimer = new Timer(DeleteOld, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(3));

            return Task.CompletedTask;
        }


        private void DoWork(object state)
        {
            if (isRunning)
                return;

            Task.Run(async () =>
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

                        if (!(dbContext.PageData.FirstOrDefault()?.IsTransactionsSaved ?? false))
                            return;

                        isRunning = true;

                        _lastBlockNumber = (int)(await _explorer.GetLastAvailableBlockNumber()).Value;

                        if (_lastCheckedBlockNumber == 0)
                            _lastCheckedBlockNumber = (int) (dbContext.BlockChainTransactions
                                .Max(w => w.BlockNumber));

                        if (_lastCheckedBlockNumber < _lastBlockNumber)
                        {
                            var transactions = _explorer.GetLatestTransactions(_lastCheckedBlockNumber,
                                _lastCheckedBlockNumber);

                            dbContext.BlockChainTransactions.AddRange(transactions);
                            dbContext.SaveChanges();
                            _lastCheckedBlockNumber++;
                        }

                        isRunning = false;

                    }
                }
                catch (Exception e)
                {

                }
            });           
        }

        private void DeleteOld(object state)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

                    var forDelete =
                        dbContext.BlockChainTransactions.Where(t => t.BlockNumber < (_lastCheckedBlockNumber - Helpers.Constants.Ints.BlocksCount.SaveBlocksCount)).ToList();

                    foreach (var blockChainTransaction in forDelete)
                    {
                        dbContext.BlockChainTransactions.Remove(blockChainTransaction);
                    }

                    dbContext.SaveChanges();

                }
            }
            catch (Exception e)
            {

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _deleteTimer?.Change(Timeout.Infinite, 0);
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
            _deleteTimer?.Dispose();
        }
    }
}
