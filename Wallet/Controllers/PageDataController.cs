using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wallet.Models;

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    public class PageDataController : Controller
    {

        private WalletDbContext dbContext;

        public PageDataController(WalletDbContext context)
        {
            this.dbContext = context;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPageElements()
        {
            try
            {
                return new OkObjectResult(await dbContext.PageData.ToListAsync());
            }
            catch (Exception e)
            {
                return StatusCode(500, $"En error occurred :{e.Message}");
            }
        }

        [HttpPost("[action]")]
        [Authorize(Policy = "ApiAdmin")]
        public async Task<IActionResult> SetPageElements([FromBody] PageData[] model)
        {
            try
            {
                dbContext.PageData.UpdateRange(model);
                await dbContext.SaveChangesAsync();
                return new OkResult();
            }
            catch (Exception e)
            {
                return BadRequest($"En error occurred :{e.Message}");
            }
        }
    }
}