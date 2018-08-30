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
            int minBlock,maxBlock,tmp;
            
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
                        if (minBlock >0 && minBlock< tmp)
                        {
                            var data = dbContext.BlockChainTransactions.Where(t => t.BlockNumber < tmp);
                            foreach (var t in data )
                            {
                                dbContext.BlockChainTransactions.Remove(t);
                            }
                        }

                        if (maxBlock >0 && maxBlock< _lastBlockNumber)
                        {
                            //add meth
                        }
                        dbContext.SaveChanges();

                    }
                }
                catch (Exception e)
                {

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
