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
                if (token.Address != accountAddress) //finish with module 5, check ico
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
        public async Task<IActionResult> GetTokenByName(string tokenName)
        {
            try
            {
                var token = await dbContext.Erc20Tokens.FirstOrDefaultAsync(t =>
                    t.Name.Contains(tokenName.Trim(), StringComparison.CurrentCultureIgnoreCase));

                token.Quantity = Web3.Convert.FromWei(await _explorer.GetTokenTotalSupply(token.Address),
                    token.DecimalPlaces);

                return new OkObjectResult(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTokenByParameters(string tokenParams)
        {
            int res;

            try
            {
                List<ERC20Token> token = null;
                bool isDecimal = Int32.TryParse(tokenParams, out res);
                if (res != 0)
                {
                    //token= await dbContext.Erc20Tokens.FindAsync( (pr => pr.DecimalPlaces == res) );
                    //token = await dbContext.Erc20Tokens.FindAsync((pr => pr.DecimalPlaces == res));
                }
                //выгружать пользователю список всего, где встретилось значение???


                return new OkObjectResult(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> IsContract(string address)
        {
            try
            {
                string result = await _explorer.GetCode(address);

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

                return new OkObjectResult(
                    new TransactionsViewModel()
                    {
                        BlockNumber = searchBlockNumber,
                        Transactions = result.OrderByDescending(t => t.Date).ToList()
                    }
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
            try
            {
                // if didnt have in db sometime error, bcause try to get data in one time
                Task<ERC20Token> getContractInfoFromDb = Task.Run(async () =>
                {
                    var token = await dbContext.Erc20Tokens.FirstOrDefaultAsync(t =>
                        t.Address.Equals(contractAddress, StringComparison.CurrentCultureIgnoreCase));
                    token = token == null ? token = new ERC20Token() { Address = contractAddress } : token;
                    token.Quantity = Web3.Convert.FromWei(await _explorer.GetTokenTotalSupply(token.Address),
                        token.DecimalPlaces);
                    return token;
                });

                Task<ContractHoldersAndTransactionsModel> getHoldersAndTransactions = Task.Run(async () =>
                {
                    var token1 = await dbContext.Erc20Tokens.FirstOrDefaultAsync(t =>
                        t.Address.Equals(contractAddress, StringComparison.CurrentCultureIgnoreCase));

                    //token = token == null ? token = new ERC20Token() { Address = contractAddress }:token;


                    ContractHoldersAndTransactionsModel tmp;
                    //tmp = await Parser.GetContractHoldersAndTransactions(contractAddress);
                    if (token1 == null)
                    {
                        token1 = new ERC20Token() { Address = contractAddress };
                        tmp = await Parser.GetContractHoldersAndTransactions(contractAddress);

                        token1.TransactionsCount = tmp.TransactionsCount;
                        token1.WalletsCount = tmp.HoldersCount;
                        token1.UpdDate = DateTime.Now;
                        dbContext.Erc20Tokens.Add(token1);
                        await dbContext.SaveChangesAsync();

                    }
                    else
                    {
                        if (token1.UpdDate == null || (((TimeSpan)(DateTime.Now - token1.UpdDate)).Minutes > 10))
                        {
                            tmp = await Parser.GetContractHoldersAndTransactions(contractAddress);

                            token1.TransactionsCount = tmp.TransactionsCount;
                            token1.WalletsCount = tmp.HoldersCount;
                            token1.UpdDate = DateTime.Now;
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            tmp = new ContractHoldersAndTransactionsModel();
                            tmp.HoldersCount = token1.WalletsCount;
                            tmp.TransactionsCount = token1.TransactionsCount;
                        }
                    }


                    return tmp;
                });

                await Task.WhenAll(getContractInfoFromDb, getHoldersAndTransactions);
                var result = getContractInfoFromDb.Result;
                result.TransactionsCount = getHoldersAndTransactions.Result.TransactionsCount;
                result.WalletsCount = getHoldersAndTransactions.Result.HoldersCount;

                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSmartContractTransactions(int? blockNumber, string accountAddress)
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
                //check by time updates && update db count of transaction "v.	Quantity of transactions"

                //uniq count of wallet "iv.	Quantity of wallets "

                //iii.	Quantity "sold count"???

                var tasks = new List<Task<List<CustomTransaction>>>();
                for (int i = searchBlockNumber - 100; i <= searchBlockNumber; i++)
                {
                    Task<List<CustomTransaction>> task = _explorer.GetSmartContractTransactions(accountAddress, i);
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks.ToArray());

                var result = new List<CustomTransaction>();
                foreach (var listtask in tasks)
                {
                    result.AddRange(listtask.Result);
                }

                return new OkObjectResult(
                    new TransactionsViewModel()
                    {
                        BlockNumber = searchBlockNumber,
                        Transactions = result.OrderByDescending(t => t.Date).ToList()
                    }
                );
            }
            catch (Exception e)
            {
                return BadRequest(HttpErrorHandler.AddError("Failure", e.Message, ModelState));
            }
        }
    }
}