using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wallet.Helpers;
using Wallet.Models;
using Wallet.ViewModels;

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    public class TokenDataController : Controller
    {
        private WalletDbContext _dbContext;


        public TokenDataController(WalletDbContext context)
        {          
            _dbContext = context;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTokenlist()
        {
            try
            {
                var data = await _dbContext.Erc20Tokens.ToListAsync();

                return new ObjectResult(data);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"En error occurred :{e.Message}");
            }
        }
    }
}