using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Wallet.Models
{
    public class WalletDbContext:IdentityDbContext<User>
    {
        public DbSet<ERC20Token> Erc20Tokens { get; set; }
        public DbSet<UserWatchlist> UserWatchlist { get; set; }
        public DbSet<PageData> PageData { get; set; }
        public DbSet<SmartContract> SmartContracts { get; set; }

        public WalletDbContext(DbContextOptions<WalletDbContext> options)
            : base(options)
        {
        }
    }
}