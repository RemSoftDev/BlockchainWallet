using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wallet.BlockchainAPI.Model;

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
        public DbSet<ChainBlock> ChainBlock { get; set; }
        public DbSet<ChainTransaction> ChainTransaction { get; set; }
        public DbSet<TransactionMod> CustomTransaction { get; set; }
        public DbSet<LastWalletBlock> LastWalletBlocks { get; set; }

        public WalletDbContext(DbContextOptions<WalletDbContext> options)
            : base(options)
        {
        }
    }
}