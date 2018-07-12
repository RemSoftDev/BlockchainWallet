using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Wallet.BlockchainAPI;
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
                var userManager = scopeServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scopeServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                db.Database.Migrate();
                await InsertTokens(db);
                await InsertPageElements(db);
                await InsertUser(db, userManager, roleManager);
            }
        }

        private static async Task InsertUser(WalletDbContext context, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if ((await context.Users.FirstOrDefaultAsync(u=>u.Email == "blowaud@gmail.com"))!= null)
                return;

            const string email = "blowaud@gmail.com";
            const string role = "Admin";
            await CreateDefaultAdministratorRole(roleManager, role);
            var user = await CreateDefaultUser(userManager, email);
            await AddDefaultRoleToDefaultUser(userManager, role, user);
            user.EmailConfirmed = true;
            await context.SaveChangesAsync();
        }

        private static async Task CreateDefaultAdministratorRole(RoleManager<IdentityRole> roleManager, string role)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }

        private static async Task<User> CreateDefaultUser(UserManager<User> userManager, string email)
        {
            var user = new User(){ UserName = email, Email = email};

            await userManager.CreateAsync(user, "blowaud1234");

            var createdUser = await userManager.FindByEmailAsync(email);
            return createdUser;
        }

        private static async Task AddDefaultRoleToDefaultUser(UserManager<User> userManager, string adminRole, User user)
        {
            await userManager.AddToRoleAsync(user, adminRole);
        }

        private static async Task InsertPageElements(WalletDbContext context)
        {
            if (context.PageData.Any())
                return;
            var data = new List<PageData>()
            {
                new PageData() {ElementName = "AboutPage", ElementData = "Test data for about page"},
                new PageData() {ElementName = "ContactPage", ElementData = "Test data for contact page"},
                new PageData() {ElementName = "TipsETH", ElementData = "0x53d284357ec70ce289d6d64134dfac8e511c8a3d"},
                new PageData() {ElementName = "TipsBTC", ElementData = "0x742d35cc6634c0532925a3b844bc454e4438f44e"}
            };
            context.PageData.AddRange(data);
            await context.SaveChangesAsync();
        }

        private static async Task InsertTokens(WalletDbContext context)
        {
            if (context.Erc20Tokens.Any())
                return;
            var tokens = ERC20TokensData.GetTokens();
            context.Erc20Tokens.AddRange(tokens);
            await context.SaveChangesAsync();
        }
    }
}