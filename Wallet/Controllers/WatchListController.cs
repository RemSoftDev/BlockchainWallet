using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
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
        public async Task<IActionResult> GetBlocks()
        {
            try
            {
                bool rec = false;
                var blocks = await _dbContext.ChainBlock.ToListAsync();
                var count = _dbContext.ChainTransaction.Count();
                var transactions = await _dbContext.ChainTransaction
                    .Where(t => t.From.Equals("0x3f5CE5FBFe3E9af3971dD833D26bA9b5C936f0bE", StringComparison.CurrentCultureIgnoreCase) || 
                                t.From.Equals("0x514910771af9ca656af840dff83e8264ecf986ca", StringComparison.CurrentCultureIgnoreCase))
                    .ToListAsync();
                bool res = false;
            }
            catch (Exception e)
            {
               
            }

            //Web3 web3 = new Web3();
            //for (var i = 6000001; i <= 6010000; i += 1)
            //{
            //    try
            //    {
            //        var block = web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(
            //            new HexBigInteger(i)).Result;
            //        List<ChainTransaction> transact = new List<ChainTransaction>();
            //        var numb = (int)block.Number.Value;
            //        var time = (long)block.Timestamp.Value;
            //        ChainBlock chain = new ChainBlock()
            //        {
            //            Number = numb,
            //            TimeStamp = time,
            //        };
            //        _dbContext.ChainBlock.Add(chain);
            //        foreach (var blockTransaction in block.Transactions)
            //        {
            //            transact.Add(new ChainTransaction()
            //            {
            //                From = blockTransaction.From,
            //                To = blockTransaction.To,
            //                Value = Web3.Convert.FromWei(blockTransaction.Value, 18),
            //                IsSuccess = true,
            //                BlockNumber = (int)block.Number.Value,
            //                ChainBlockId = chain.Id
            //            });
            //        }

            //        _dbContext.ChainTransaction.AddRange(transact);
            //        _dbContext.SaveChanges();
            //    }
            //    catch (Exception e)
            //    {
            //        i--;
            //    }   
            //}
            return Ok();
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

                var watchList = _dbContext.UserWatchlist.Add(new UserWatchlist()
                {
                    Address = model.Address,
                    UserEmail = model.UserEmail,
                    IsContract = model.IsContract
                });

                model.NotificationOptions.UserWatchlistId = watchList.Entity.Id;

                _dbContext.NotificationOptions.Add(model.NotificationOptions);

                await _dbContext.SaveChangesAsync();

                return new OkResult();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"En error occurred :{e.Message}");
            }       
        }
    }
}