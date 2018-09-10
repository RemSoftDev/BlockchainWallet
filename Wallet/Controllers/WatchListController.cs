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

        [HttpGet("[action]")]
        public async Task<IActionResult> GetNotificationOptions(string address)
        {
            var watchlistModel = await _dbContext.UserWatchlist.Where(w =>
                w.Address.Equals(address, StringComparison.CurrentCultureIgnoreCase)).Include(w=>w.NotificationOptions).FirstOrDefaultAsync();

            if (watchlistModel != null)
            {
                NotificationOptions result = ReplaceTokenAddresses(watchlistModel.NotificationOptions);
                result.UserWatchlist = null;
                return new OkObjectResult(result);
            }

            return NotFound();
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

                if (data.Count == 0)
                {
                    var watchList = _dbContext.UserWatchlist.Add(new UserWatchlist()
                    {
                        Address = model.Address,
                        UserEmail = model.UserEmail,
                        IsContract = model.IsContract,
                        TokenDecimalPlaces = model.TokenDecimalPlaces
                    });
                
                    model.NotificationOptions.UserWatchlistId = watchList.Entity.Id;

                    model.NotificationOptions = ReplaceTokenSymbols(model.NotificationOptions);

                    _dbContext.NotificationOptions.Add(model.NotificationOptions);

                    await _dbContext.SaveChangesAsync();

                    return new OkResult();
                }
                var options  = ReplaceTokenSymbols(model.NotificationOptions);

                _dbContext.NotificationOptions.Update(options);

                await _dbContext.SaveChangesAsync();

                return new OkResult();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"En error occurred :{e.Message}");
            }       
        }

        private NotificationOptions ReplaceTokenAddresses(NotificationOptions options)
        {
            if (!string.IsNullOrEmpty(options.TokenOrEtherSentName) && !options.TokenOrEtherSentName.Equals("ETH"))
            {
                options.TokenOrEtherSentName = _dbContext.Erc20Tokens
                    .FirstOrDefault(t => t.Address.Equals(options.TokenOrEtherSentName,
                        StringComparison.CurrentCultureIgnoreCase))
                    ?.Symbol;
            }
            if (!string.IsNullOrEmpty(options.NumberOfTokenOrEtherWasSentName) && !options.NumberOfTokenOrEtherWasSentName.Equals("ETH"))
            {
                options.NumberOfTokenOrEtherWasSentName = _dbContext.Erc20Tokens
                    .FirstOrDefault(t => t.Address.Equals(options.NumberOfTokenOrEtherWasSentName,
                        StringComparison.CurrentCultureIgnoreCase))
                    ?.Symbol;
            }
            if (!string.IsNullOrEmpty(options.TokenOrEtherReceivedName) && !options.TokenOrEtherReceivedName.Equals("ETH"))
            {

                options.TokenOrEtherReceivedName = _dbContext.Erc20Tokens
                    .FirstOrDefault(t => t.Address.Equals(options.TokenOrEtherReceivedName,
                        StringComparison.CurrentCultureIgnoreCase))
                    ?.Symbol;
            }
            if (!string.IsNullOrEmpty(options.TokenOrEtherWasReceivedName) && !options.TokenOrEtherWasReceivedName.Equals("ETH"))
            {
                options.TokenOrEtherWasReceivedName = _dbContext.Erc20Tokens
                    .FirstOrDefault(t => t.Address.Equals(options.TokenOrEtherWasReceivedName,
                        StringComparison.CurrentCultureIgnoreCase))
                    ?.Symbol;
            }

            return options;
        }

        private NotificationOptions ReplaceTokenSymbols(NotificationOptions options)
        {
            if (!string.IsNullOrEmpty(options.TokenOrEtherSentName) && !options.TokenOrEtherSentName.Equals("ETH"))
            {
                options.TokenOrEtherSentName = _dbContext.Erc20Tokens
                    .FirstOrDefault(t => t.Symbol.Equals(options.TokenOrEtherSentName,
                        StringComparison.CurrentCultureIgnoreCase))
                    ?.Address;
            }
            if (!string.IsNullOrEmpty(options.NumberOfTokenOrEtherWasSentName) && !options.NumberOfTokenOrEtherWasSentName.Equals("ETH"))
            {
                var token = _dbContext.Erc20Tokens
                    .FirstOrDefault(t => t.Symbol.Equals(options.NumberOfTokenOrEtherWasSentName,
                        StringComparison.CurrentCultureIgnoreCase));
                options.NumberOfTokenOrEtherWasSentName = token?.Address;
                options.TokenSentDecimalPlaces = token?.DecimalPlaces ?? 18;
            }
            if (!string.IsNullOrEmpty(options.TokenOrEtherReceivedName) && !options.TokenOrEtherReceivedName.Equals("ETH"))
            {

                options.TokenOrEtherReceivedName = _dbContext.Erc20Tokens
                    .FirstOrDefault(t => t.Symbol.Equals(options.TokenOrEtherReceivedName,
                        StringComparison.CurrentCultureIgnoreCase))
                    ?.Address;
            }
            if (!string.IsNullOrEmpty(options.TokenOrEtherWasReceivedName) && !options.TokenOrEtherWasReceivedName.Equals("ETH"))
            {
                var token = _dbContext.Erc20Tokens
                    .FirstOrDefault(t => t.Symbol.Equals(options.TokenOrEtherWasReceivedName,
                        StringComparison.CurrentCultureIgnoreCase));
                options.TokenOrEtherWasReceivedName = token?.Address;
                options.TokenReceivedDecimalPlaces = token?.DecimalPlaces ?? 18;
            }

            return options;
        }
    }
}