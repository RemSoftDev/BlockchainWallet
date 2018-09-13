using Microsoft.EntityFrameworkCore;
using Wallet.Models;

namespace Wallet.Helpers
{
    public static class DbContextOptionsFactory
    {
        public static DbContextOptions<WalletDbContext> DbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<WalletDbContext>();
            optionsBuilder.UseSqlServer(Startup.ConnectionString);

            return optionsBuilder.Options;
        }
    }
}