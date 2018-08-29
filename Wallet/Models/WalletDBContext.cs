using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Wallet.Models
{
    public class WalletDbContext:IdentityDbContext<User>
    {
        public DbSet<ERC20Token> Erc20Tokens { get; set; }
        public DbSet<UserWatchlist> UserWatchlist { get; set; }
        public DbSet<PageData> PageData { get; set; }
        public DbSet<NotificationOptions> NotificationOptions { get; set; }
        public DbSet<CustomEventLog> CustomEventLogs { get; set; }
        public DbSet<TokenHolder> TokenHolders { get; set; }
        public DbSet<BlockChainTransaction> BlockChainTransactions { get; set; }

        public WalletDbContext(DbContextOptions<WalletDbContext> options)
            : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal)))
            {
                property.Relational().ColumnType = "decimal(28, 10)";
            }
        }
    }
}