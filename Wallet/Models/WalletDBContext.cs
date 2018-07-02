using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Wallet.Models
{
    public class WalletDbContext:IdentityDbContext<User>
    {
        public DbSet<ERC20Token> Erc20Tokens { get; set; }

        public WalletDbContext(DbContextOptions<WalletDbContext> options)
            : base(options)
        {
        }
    }
}