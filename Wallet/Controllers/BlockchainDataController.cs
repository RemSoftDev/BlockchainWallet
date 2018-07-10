using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.Hex.HexConvertors;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Wallet.BlockchainAPI;
using Wallet.BlockchainAPI.Model;
using Wallet.Models;
using Wallet.ViewModels;
using Nethereum.ABI;
using Wallet.Helpers;

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    public class BlockchainDataController : Controller
    {
        private IBlockchainExplorer _explorer;
        private WalletDbContext dbContext;

        public BlockchainDataController(IBlockchainExplorer explorer, WalletDbContext context)
        {
            this._explorer = explorer;
            this.dbContext = context;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetWalletInfo(string accountAddress)
        {
            WalletInfoViewModel model = new WalletInfoViewModel();
            Task<HexBigInteger> getBalance = _explorer.BalanceETH(accountAddress);

            var listtasks = new List<Task<ERC20TokenViewModel>>();
            List<ERC20TokenViewModel> tokens = new List<ERC20TokenViewModel>();

            foreach (var token in dbContext.Erc20Tokens)
            { 
                if (token.Address != accountAddress)//finish with module 5, check ico
                {
                    Task<ERC20TokenViewModel> task = _explorer.BalanceToken(token, accountAddress);
                    listtasks.Add(task);
                }
                else
                {
                    break;
                }
            }

            try
            {
                model.Balance = Web3.Convert.FromWei(await getBalance, 18);

                await Task.WhenAll(listtasks.ToArray());
                foreach (var listtask in listtasks)
                {
                    tokens.Add(listtask.Result);
                }
                model.Tokens = tokens.Where(x => x.Balance != 0).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(HttpErrorHandler.AddError("Failure", e.Message, ModelState));
            }

            return new OkObjectResult(model);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTransactions(string accountAddress)
        {
            try
            {
                var result = new List<CustomTransaction>();
                var lastblockNumber = await _explorer.GetLastAvailableBlockNumber();
                var listtasks = new List<Task<List<CustomTransaction>>>();
                for (int i = (int)lastblockNumber.Value - 50; i <= lastblockNumber.Value; i++)
                {
                    Task<List<CustomTransaction>> task = _explorer.GetTransactions(accountAddress, i);
                    listtasks.Add(task);
                }

                await Task.WhenAll(listtasks.ToArray());

                foreach (var listtask in listtasks)
                {
                    result.AddRange(listtask.Result);
                }

                result.ForEach(t =>
                {
                    if (t.Value.Value == 0)
                    {
                        var decodedInput = DecodeInput(t.Input, t.ContractAddress);
                        t.To = decodedInput.To;
                        t.What = decodedInput.What;
                        t.DecimalValue = decodedInput.Value;
                        t.ContractAddress = decodedInput.ContractAddress;
                    }
                    else
                    {
                        t.DecimalValue = Web3.Convert.FromWei(t.Value.Value, 18);
                    }
                });
                return new OkObjectResult(result.OrderByDescending(t => t.Date).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(HttpErrorHandler.AddError("Failure", e.Message, ModelState));
            }
        }

        private TransactionInput DecodeInput(string input, string contractAddress)
        {
            HexBigIntegerBigEndianConvertor a = new HexBigIntegerBigEndianConvertor();
            //cut off method name (first 4 byte)
            input = input.Substring(10);
            //get value         
            var value = a.ConvertFromHex(input.Substring(input.Length / 2));
            //get address
            var address = a.ConvertToHex(a.ConvertFromHex("0x" + input.Substring(0, input.Length / 2)));

            var token = dbContext.Erc20Tokens.FirstOrDefault(t =>
                string.Equals(t.Address, contractAddress, StringComparison.CurrentCultureIgnoreCase));
            return new TransactionInput()
            {
                To = address,
                Value = token != null ? Web3.Convert.FromWei(value, token.DecimalPlaces) : (decimal)value,
                What = token?.Symbol ?? "Unknown",
                ContractAddress = token?.Address ?? "Unknown"
            };
        }
    }
}