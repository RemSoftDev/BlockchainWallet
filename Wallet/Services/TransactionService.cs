using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
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
        private bool _isRunning;
        private bool _isDeleting;

        public TransactionService(IBlockchainExplorer explorer, IServiceScopeFactory scopeFactory)
        {
            _explorer = explorer;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(4));

            _deleteTimer = new Timer(DeleteOld, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(3));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            if (_isRunning)
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

                        _isRunning = true;

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

                        _isRunning = false;

                    }
                }
                catch (Exception e)
                {
                    _isRunning = false;
                }
            });           
        }

        private void DeleteOld(object state)
        {
            try
            {
                if (_isDeleting)
                    return;

                _isDeleting = true;
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

                    var lastBlockNumber = dbContext.BlockChainTransactions
                        .Max(w => w.BlockNumber);

                    var forDelete =
                        dbContext.BlockChainTransactions.Where(t => 
                            t.BlockNumber < (lastBlockNumber - Helpers.Constants.Ints.BlocksCount.SaveBlocksCount)).Take(10000);

                    dbContext.BlockChainTransactions.RemoveRange(forDelete);
                    
                    dbContext.SaveChanges();
                }

                _isDeleting = false;
            }
            catch (Exception e)
            {
                _isDeleting = false;
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
