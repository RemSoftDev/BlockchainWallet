using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wallet.BlockchainAPI;
using Wallet.Models;
using Wallet.Notifications;

namespace Wallet.Services
{
    public class TransactionService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IBlockchainExplorer _explorer;
        private Timer _timer;
        private int _lastCheckedBlockNumber;
        private int _lastBlockNumber;




        public TransactionService(IBlockchainExplorer explorer, IServiceScopeFactory scopeFactory)
        {
            _explorer = explorer;
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
            int minBlock, maxBlock, tmp;

            Task.Run(async () =>
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
                        _lastBlockNumber = (int)(await _explorer.GetLastAvailableBlockNumber()).Value;
                        minBlock = (int)(dbContext.BlockChainTransactions
                                   .Min(w => w.BlockNumber));
                        maxBlock = (int)(dbContext.BlockChainTransactions
                                   .Max(w => w.BlockNumber));
                        tmp = _lastBlockNumber - 5000;
                        if (minBlock > 0 && minBlock < tmp)
                        {
                            var data = dbContext.BlockChainTransactions.Where(t => t.BlockNumber < tmp);
                            foreach (var t in data)
                            {
                                dbContext.BlockChainTransactions.Remove(t);
                            }
                        }

                        if (maxBlock > 0 && maxBlock < _lastBlockNumber)
                        {
                            //var a = await SaveLatestTransactions(_lastBlockNumber, maxBlock, dbContext);

                        }
                        dbContext.SaveChanges();

                    }
                }
                catch (Exception e)
                {

                }
            });
        }


        public async Task<int> SaveLatestTransactions(int lastKnownBlockNumber, int maxBlockDb, WalletDbContext context)
        {
            try
            {
                int substract = 5000;
                substract = (lastKnownBlockNumber - maxBlockDb) < 5000 ? (lastKnownBlockNumber - maxBlockDb) : 5000;

                var tasks = new List<Task<List<BlockChainTransaction>>>();
                for (int i = lastKnownBlockNumber - substract; i < lastKnownBlockNumber; i += 100)
                {
                    var i1 = i;
                    var task = Task.Run(() => _explorer.GetLatestTransactions(i1, i1 + 99));
                    tasks.Add(task);
                }
                
                await Task.WhenAll(tasks);

                //debug only for this point
                var result = new List<BlockChainTransaction>();
                foreach (var task in tasks)
                {
                    task.Result.ForEach(t => result.Add(t));
                }

                context.ChangeTracker.AutoDetectChangesEnabled = false;

                var tempList = new List<BlockChainTransaction>();
                foreach (var transact in result)
                {
                    tempList.Add(transact);
                    if (tempList.Count == 100)
                    {
                        try
                        {
                            context.BlockChainTransactions.AddRange(tempList);
                            context.SaveChanges();
                            tempList.Clear();
                        }
                        catch (Exception e)
                        {
                            tempList.Clear();
                        }
                    }
                }
                context.ChangeTracker.AutoDetectChangesEnabled = true;
                return 1;
            }
            catch(Exception e)
            {
                return 0;
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
