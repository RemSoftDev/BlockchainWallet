using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Wallet.BlockchainAPI.Model;
using Wallet.Models;

namespace Wallet
{
    public static class DBInitializer
    {
        public static async Task InitializeAsync(IServiceProvider service)
        {
            using (var serviceScope = service.CreateScope())
            {
                var scopeServiceProvider = serviceScope.ServiceProvider;
                var db = scopeServiceProvider.GetService<WalletDbContext>();
                db.Database.Migrate();
                await InsertTestData(db);
            }
        }

        private static async Task InsertTestData(WalletDbContext context)
        {
            if (context.Erc20Tokens.Any())
                return;
            var tokens = getTokens();
            context.Erc20Tokens.AddRange(tokens);
            await context.SaveChangesAsync();
        }

        public static List<ERC20Token> getTokens()
        {
            string ethTokens = System.IO.File.ReadAllText("./BlockchainAPI/ethTokens.json");
            return JsonConvert.DeserializeObject<List<ERC20Token>>(ethTokens);
        }
    }
}