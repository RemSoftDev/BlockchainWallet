using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wallet.Helpers;
using Wallet.Models;
using Wallet.ViewModels;


namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    public class WatchListController : Controller
    {
        private readonly UserManager<User> _userManager;
        private WalletDbContext _dbContext;

        public WatchListController(UserManager<User> userManager, WalletDbContext context)
        {
            _userManager = userManager;
            _dbContext = context;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserWatchlist(string userEmail)
        {
            try
            {
                var data = await _dbContext.UserWatchlist
                    .Where(w => w.UserEmail.Equals(userEmail, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();

                var result = WatchlistHelper.OrganizeData(data);

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

            _dbContext.UserWatchlist.Attach(wl);
            _dbContext.UserWatchlist.Remove(wl);

            await _dbContext.SaveChangesAsync();

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddToWatchlist([FromBody]UserWatchListViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var data = _dbContext.UserWatchlist
                   .Where(w => w.UserEmail.Equals(model.UserEmail, StringComparison.CurrentCultureIgnoreCase) && w.Address == model.Address).ToList();

                if (data != null)
                {
                    var watchList = _dbContext.UserWatchlist.Add(new UserWatchlist()
                    {
                        Address = model.Address,
                        UserEmail = model.UserEmail,
                        IsContract = model.IsContract
                    });
                
                model.NotificationOptions.UserWatchlistId = watchList.Entity.Id;

                _dbContext.NotificationOptions.Add(model.NotificationOptions);

                await _dbContext.SaveChangesAsync();
                }
                return new OkResult();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"En error occurred :{e.Message}");
            }       
        }
    }
}