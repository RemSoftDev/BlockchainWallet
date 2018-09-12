using Microsoft.EntityFrameworkCore;
using Wallet.Models;

namespace Wallet.Helpers
{
    public static class DbContextOptionsFactory
    {
        public static DbContextOptions<WalletDbContext> DbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<WalletDbContext>();
            optionsBuilder.UseSqlServer("Server=WIN-2J5M9KDIB2V\\SQLEXPRESS;Initial Catalog=WalletDB;User Id=walletbc;Password=strongpas;MultipleActiveResultSets=true");

            return optionsBuilder.Options;
        }
    }
}