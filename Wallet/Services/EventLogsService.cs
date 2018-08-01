using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public EventLogsService(IBlockchainExplorer explorer, IServiceScopeFactory scopeFactory)
        {
            _explorer = explorer;

            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                //var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
                //var token = dbContext.Erc20Tokens.FirstOrDefault(t => t.Name.Equals("Tronix"));

                //var logs = _explorer.GetEventLogs(token).Result;

                //var holders = EventLogsExplorer.GetInfoFromLogs(logs);

                //for (int i = 0; i < holders.Count; i++)
                //{
                //    try
                //    {
                //        var balance = _explorer.GetTokenHolderBalance(holders[i].Address, token.Address).Result;
                //        holders[i].Quantity = Web3.Convert.FromWei(balance, token.DecimalPlaces);
                //        holders[i].ERC20TokenId = token.Id;
                //    }
                //    catch (Exception e)
                //    {
                //        i--;
                //    }

                //}

                //dbContext.CustomEventLogs.AddRange(logs);
                //dbContext.TokenHolders.AddRange(holders);
                //dbContext.SaveChanges();


                //var tokens = dbContext.Erc20Tokens.Include(t => t.Logs).ToList();

                //foreach (var token in tokens )
                //{
                //    try
                //    {
                //        if (token.Logs.Count ==0)
                //        {
                //            var logs = _explorer.GetEventLogs(token).Result;
                //            dbContext.CustomEventLogs.AddRange(logs);
                //            dbContext.SaveChanges();
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //    }
                //}
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