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

            var tasks = new List<Task<ERC20TokenViewModel>>();
            List<ERC20TokenViewModel> tokens = new List<ERC20TokenViewModel>();

            foreach (var token in dbContext.Erc20Tokens)
            { 
                if (token.Address != accountAddress)//finish with module 5, check ico
                {
                    Task<ERC20TokenViewModel> task = _explorer.BalanceToken(token, accountAddress);
                    tasks.Add(task);
                }
                else
                {
                    break;
                }
            }

            try
            {
                model.Balance = Web3.Convert.FromWei(await getBalance, 18);

                await Task.WhenAll(tasks.ToArray());
                foreach (var listtask in tasks)
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
        public async Task<IActionResult> IsContract(string address)
        {  
            try
            {
                string result =  await _explorer.GetCode(address);
               
                return new OkObjectResult(result != Constants.Strings.WalletCode.AccountCode);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTransactions(int? blockNumber, string accountAddress)
        {
            try
            {
                int searchBlockNumber;
                if (blockNumber.HasValue)
                {
                    searchBlockNumber = blockNumber.Value;
                }
                else
                {
                    searchBlockNumber = (int)(await _explorer.GetLastAvailableBlockNumber()).Value;
                }

                var tasks = new List<Task<List<CustomTransaction>>>();
                for (int i = searchBlockNumber - 100; i <= searchBlockNumber; i++)
                {
                    Task<List<CustomTransaction>> task = _explorer.GetTransactions(accountAddress, i);
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks.ToArray());

                var result = new List<CustomTransaction>();
                foreach (var listtask in tasks)
                {
                    result.AddRange(listtask.Result);
                }

                result.ForEach(t =>
                {
                    if (t.Value.Value == 0)
                    {
                        var decodedInput = InputDecoder.DecodeInput(t.Input, t.ContractAddress, dbContext);
                        if (decodedInput.ContractAddress != string.Empty)
                        {
                            t.To = decodedInput.To;
                            t.What = decodedInput.What;
                            t.DecimalValue = decodedInput.Value;
                            t.ContractAddress = decodedInput.ContractAddress;
                        }
                    }
                    else
                    {
                        t.DecimalValue = Web3.Convert.FromWei(t.Value.Value, 18);
                    }
                });
                return new OkObjectResult(
                    new TransactionsViewModel() { BlockNumber = searchBlockNumber, Transactions = result.OrderByDescending(t => t.Date).ToList() }
                    );
            }
            catch (Exception e)
            {
                return BadRequest(HttpErrorHandler.AddError("Failure", e.Message, ModelState));
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSmartContractInfo(string contractAddress)
        {
            var contract =  await dbContext.SmartContracts.FirstOrDefaultAsync(c =>
                c.Address.Equals(contractAddress, StringComparison.CurrentCultureIgnoreCase));
            return new OkObjectResult(contract);
        }

    }
}