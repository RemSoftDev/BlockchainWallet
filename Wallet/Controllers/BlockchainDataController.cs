using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
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
        private IParser _parser;
        private WalletDbContext dbContext;

        public BlockchainDataController(IBlockchainExplorer explorer, IParser parser, WalletDbContext context)
        {
            this._explorer = explorer;
            this._parser = parser;
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
        public async Task<IActionResult> GetTokenHoldersInfo(int skipElementsCount, int contractId, SortOrder sortOrder = SortOrder.QuantityDesc)
        {
            try
            {
                var holders = dbContext.TokenHolders.Where(h => h.ERC20TokenId == contractId);

                switch (sortOrder)
                {
                    case SortOrder.QuantityDesc:
                        holders = holders.OrderByDescending(h => h.Quantity);
                        break;
                    case SortOrder.Quantity:
                        holders = holders.OrderBy(h => h.Quantity);
                        break;
                    case SortOrder.TokensSent:
                        holders = holders.OrderBy(h => h.TokensSent);
                        break;
                    case SortOrder.TokensSentDesc:
                        holders = holders.OrderByDescending(h => h.TokensSent);
                        break;
                    case SortOrder.TokensReceived:
                        holders = holders.OrderBy(h => h.TokensReceived);
                        break;
                    case SortOrder.TokensReceivedDesc:
                        holders = holders.OrderByDescending(h => h.TokensReceived);
                        break;
                    case SortOrder.GeneralTransactionsNumber:
                        holders = holders.OrderBy(h => h.GeneralTransactionsNumber);
                        break;
                    case SortOrder.GeneralTransactionsNumberDesc:
                        holders = holders.OrderByDescending(h => h.GeneralTransactionsNumber);
                        break;
                    case SortOrder.SentTransactionsNumber:
                        holders = holders.OrderBy(h => h.SentTransactionsNumber);
                        break;
                    case SortOrder.SentTransactionsNumberDesc:
                        holders = holders.OrderByDescending(h => h.SentTransactionsNumber);
                        break;
                    case SortOrder.ReceivedTransactionsNumber:
                        holders = holders.OrderBy(h => h.ReceivedTransactionsNumber);
                        break;
                    case SortOrder.ReceivedTransactionsNumberDesc:
                        holders = holders.OrderByDescending(h => h.ReceivedTransactionsNumber);
                        break;

                }

                if (skipElementsCount == 0)
                {
                    return new OkObjectResult(await holders.Take(40).ToListAsync());
                    
                }

                var result = await holders.Skip(skipElementsCount).Take( 40).ToListAsync();

                return new OkObjectResult(
                    new TokenHoldersViewModel()
                    {
                        HoldersInfo = result,
                        SkipElementsCount = skipElementsCount
                    }
                );
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTokenHoldersInfoByDateTime(int skipElementsCount, int contractId, string secondsFrom, string secondsTo, SortOrder sortOrder = SortOrder.QuantityDesc)
        {
            try
            {
                var from = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Int64.Parse(secondsFrom));
                var to = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Int64.Parse(secondsTo));
                var logs = await dbContext.CustomEventLogs.Where(l=>l.dateTime>=from && l.dateTime<=to).ToListAsync();

                
                var holders = EventLogsExplorer.GetInfoFromLogs(logs);


                holders.ForEach(h =>
                    {
                        h.Quantity = (h.TokensReceived - h.TokensSent) > 0 ? h.TokensReceived - h.TokensSent : 0;
                    });

                var result = holders.AsQueryable();

                switch (sortOrder)
                {
                    case SortOrder.QuantityDesc:
                        result = result.OrderByDescending(h => h.Quantity);
                        break;
                    case SortOrder.Quantity:
                        result = result.OrderBy(h => h.Quantity);
                        break;
                    case SortOrder.TokensSent:
                        result = result.OrderBy(h => h.TokensSent);
                        break;
                    case SortOrder.TokensSentDesc:
                        result = result.OrderByDescending(h => h.TokensSent);
                        break;
                    case SortOrder.TokensReceived:
                        result = result.OrderBy(h => h.TokensReceived);
                        break;
                    case SortOrder.TokensReceivedDesc:
                        result = result.OrderByDescending(h => h.TokensReceived);
                        break;
                    case SortOrder.GeneralTransactionsNumber:
                        result = result.OrderBy(h => h.GeneralTransactionsNumber);
                        break;
                    case SortOrder.GeneralTransactionsNumberDesc:
                        result = result.OrderByDescending(h => h.GeneralTransactionsNumber);
                        break;
                    case SortOrder.SentTransactionsNumber:
                        result = result.OrderBy(h => h.SentTransactionsNumber);
                        break;
                    case SortOrder.SentTransactionsNumberDesc:
                        result = result.OrderByDescending(h => h.SentTransactionsNumber);
                        break;
                    case SortOrder.ReceivedTransactionsNumber:
                        result = result.OrderBy(h => h.ReceivedTransactionsNumber);
                        break;
                    case SortOrder.ReceivedTransactionsNumberDesc:
                        result = result.OrderByDescending(h => h.ReceivedTransactionsNumber);
                        break;

                }

                if (skipElementsCount == 0)
                {
                    return new OkObjectResult(result.Skip(skipElementsCount).Take(40).ToList());
                }

                return new OkObjectResult(
                        new TokenHoldersViewModel()
                        {
                            HoldersInfo = result.Skip(skipElementsCount).Take(40).ToList(),
                            SkipElementsCount = skipElementsCount
                        }                
                    );
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
                        tmp = await _parser.GetContractHoldersAndTransactions(contractAddress);

                        token1.TransactionsCount = tmp.TransactionsCount;
                        token1.WalletsCount = tmp.HoldersCount;
                        token1.UpdDate = DateTime.Now;
                        dbContext.Erc20Tokens.Add(token1);
                        dbContext.SaveChangesAsync();

                    }
                    else
                    {
                        //if (token1.UpdDate == null || (((TimeSpan)(DateTime.Now - token1.UpdDate)).Minutes > 10))
                        {
                            tmp = await _parser.GetContractHoldersAndTransactions(contractAddress);

                            token1.TransactionsCount = tmp.TransactionsCount;
                            token1.WalletsCount = tmp.HoldersCount;
                            token1.UpdDate = DateTime.Now;
                            dbContext.SaveChangesAsync();
                        }
                        //else
                        //{
                        //    tmp = new ContractHoldersAndTransactionsModel();
                        //    tmp.HoldersCount = token1.WalletsCount;
                        //    tmp.TransactionsCount = token1.TransactionsCount;
                        //}
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
        public async Task<IActionResult> GetSmartContractInfoByName(string contractName)
        {
            try
            {
                var token = dbContext.Erc20Tokens.ToList().FirstOrDefault(t=> t.Name?.Equals(contractName, StringComparison.CurrentCultureIgnoreCase)??false);

                if (token == null)
                    return NotFound();

                Task<ERC20Token> getContractQuantity = Task.Run(async () =>
                {

                    token = token == null ? token = new ERC20Token() { Name = contractName } : token;
                    token.Quantity = Web3.Convert.FromWei(await _explorer.GetTokenTotalSupply(token.Address),
                        token.DecimalPlaces);
                    return token;
                });

                Task<ContractHoldersAndTransactionsModel> getHoldersAndTransactions = Task.Run(async () =>
                {
                    var token1 = await dbContext.Erc20Tokens.FirstOrDefaultAsync(t =>
                        t.Address.Equals(token.Address, StringComparison.CurrentCultureIgnoreCase));

                    ContractHoldersAndTransactionsModel tmp;
                    if (token1 == null)
                    {
                        token1 = new ERC20Token() { Address = token.Address };
                        tmp = await _parser.GetContractHoldersAndTransactions(token.Address);

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
                            tmp = await _parser.GetContractHoldersAndTransactions(token.Address);

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

                await Task.WhenAll(getContractQuantity, getHoldersAndTransactions);
                var result = getContractQuantity.Result;
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

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSmartContractTransactionsByName(string contractName)
        {
            try
            {
                int searchBlockNumber = (int)(await _explorer.GetLastAvailableBlockNumber()).Value;
                var token = dbContext.Erc20Tokens.ToList().FirstOrDefault(t => t.Name?.Equals(contractName, StringComparison.CurrentCultureIgnoreCase) ?? false);

                if (token == null)
                    return NotFound();

                var tasks = new List<Task<List<CustomTransaction>>>();
                for (int i = searchBlockNumber - 100; i <= searchBlockNumber; i++)
                {
                    Task<List<CustomTransaction>> task = _explorer.GetSmartContractTransactions(token.Address, i);
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

    public enum  SortOrder
    {
        QuantityDesc,
        Quantity,
        TokensSent,
        TokensSentDesc,
        TokensReceived,
        TokensReceivedDesc,
        GeneralTransactionsNumber,
        GeneralTransactionsNumberDesc,
        SentTransactionsNumber,
        SentTransactionsNumberDesc,
        ReceivedTransactionsNumber,
        ReceivedTransactionsNumberDesc
    }
}