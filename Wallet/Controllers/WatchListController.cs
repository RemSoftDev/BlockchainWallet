using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wallet.Models;
using Wallet.ViewModels;


namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    public class WatchListController : Controller
    {
        private readonly UserManager<User> _userManager;
        private WalletDbContext dbContext;

        public WatchListController(UserManager<User> userManager, WalletDbContext context)
        {
            _userManager = userManager;
            this.dbContext = context;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserWatchlist(string userEmail)
        {
            try
            {
                var data = await dbContext.UserWatchlist
                    .Where(w => w.UserEmail.Equals(userEmail, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();

                var result = OrganizeData(data);

                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"En error occurred :{e.Message}");
            }

        }

        public async Task DeleteFromWatchlist(int idwatchlist)
        {
            UserWatchlist wl = new UserWatchlist
            {
                Id = idwatchlist
            };

            dbContext.UserWatchlist.Attach(wl);
            dbContext.UserWatchlist.Remove(wl);

            await dbContext.SaveChangesAsync();

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddToWatchlist([FromBody]UserWatchListViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dbContext.UserWatchlist.Add(new UserWatchlist()
                {
                    Address = model.Address,
                    UserEmail = model.UserEmail,
                    IsContract = model.IsContract
                });
                await dbContext.SaveChangesAsync();

                return new OkResult();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"En error occurred :{e.Message}");
            }       
        }

        public List<WatchlistByAccounts> OrganizeData(List<UserWatchlist> data)
        {
            var result = new List<WatchlistByAccounts>();
            var accounts = new List<UserWatchlist>();
            var contracts = new List<UserWatchlist>();

            foreach (var entry in data)
            {
                if (entry.IsContract)
                {
                    contracts.Add(entry);
                }
                else
                {
                    accounts.Add(entry);
                }
            }

            int length = contracts.Count >= accounts.Count ? contracts.Count : accounts.Count;
            if (contracts.Count > accounts.Count)
            {
                for (int i = 0; i < length - accounts.Count; i++)
                {
                    accounts.Add(new UserWatchlist() { Address = String.Empty, UserEmail = String.Empty });
                }
            }
            else
            {
                for (int i = 0; i < length - contracts.Count; i++)
                {
                    contracts.Add(new UserWatchlist() { Address = String.Empty, UserEmail = String.Empty });
                }
            }

            for (int i = 0; i < length; i++)
            {
                result.Add(new WatchlistByAccounts()
                {
                    Contract = contracts[i],
                    Account = accounts[i]
                });
            }

            return result;
        }
    }
}