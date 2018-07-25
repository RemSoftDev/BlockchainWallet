using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetSmartContract(string contractAddress)
        {
            try
            {
                var token = await _dbContext.Erc20Tokens.FirstOrDefaultAsync(t =>
                    t.Address.Equals(contractAddress, StringComparison.CurrentCultureIgnoreCase));
                return new OkObjectResult(token);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("[action]")]
        [Authorize(Policy = "ApiAdmin")]
        public async Task<IActionResult> UpdateSmartContract([FromBody] ERC20Token model)
        {
            try
            {
                _dbContext.Erc20Tokens.Update(model);
                await _dbContext.SaveChangesAsync();
                return new OkResult();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}