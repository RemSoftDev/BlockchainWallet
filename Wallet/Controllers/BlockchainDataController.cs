using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Wallet.BlockchainAPI;
using Wallet.BlockchainAPI.Model;
using Wallet.Models;
using Wallet.ViewModels;
using Wallet.Helpers;

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    public class BlockchainDataController : Controller
    {
        private IBlockchainExplorer _explorer;
        private WalletDbContext _dbContext;

        public BlockchainDataController(IBlockchainExplorer explorer, WalletDbContext context)
        {
            this._explorer = explorer;
            this._dbContext = context;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetWalletInfo(string accountAddress)
        {
            WalletInfoViewModel model = new WalletInfoViewModel();
            Task<HexBigInteger> getBalance = _explorer.BalanceETH(accountAddress);

            var tasks = new List<Task<ERC20TokenViewModel>>();
            List<ERC20TokenViewModel> tokens = new List<ERC20TokenViewModel>();

            foreach (var token in _dbContext.Erc20Tokens)
            {
                if (token.Address != accountAddress)
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
                var token = await _dbContext.Erc20Tokens.FirstOrDefaultAsync(t =>
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
        public async Task<IActionResult> IsContract(string address)
        {
            try
            {
                var result = await _explorer.GetCode(address);

                return new OkObjectResult(result != Constants.Strings.WalletCode.AccountCode);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTokenHoldersInfo(int skipElementsCount, int contractId,
            SortOrder sortOrder = SortOrder.QuantityDesc)
        {
            try
            {
                var holders = _dbContext.TokenHolders.Where(h => h.ERC20TokenId == contractId);

                holders = SortHoldersInfo(sortOrder, holders);

                if (skipElementsCount == 0)
                {
                    return new OkObjectResult(await holders.Take(40).ToListAsync());
                }

                var result = await holders.Skip(skipElementsCount).Take(40).ToListAsync();

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
        public async Task<IActionResult> GetTokenHoldersInfoByDateTime(int skipElementsCount, int contractId,
            string secondsFrom, string secondsTo, SortOrder sortOrder = SortOrder.QuantityDesc)
        {
            try
            {
                var from = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Int64.Parse(secondsFrom));
                var to = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Int64.Parse(secondsTo));

                var logs = await _dbContext.CustomEventLogs.Where(l => l.ERC20TokenId == contractId)
                    .Where(l => l.WhenDateTime >= from && l.WhenDateTime <= to).ToListAsync();

                var holders = EventLogsExplorer.GetInfoFromLogs(logs);

                holders.ForEach(h =>
                {
                    h.Quantity = (h.TokensReceived - h.TokensSent) > 0 ? h.TokensReceived - h.TokensSent : 0;
                });

                var result = SortHoldersInfo(sortOrder, holders.AsQueryable());

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
        public async Task<IActionResult> GetTransactions(int? skipElementsNumber, string accountAddress)
        {
            try
            {
                skipElementsNumber = skipElementsNumber ?? 0;

                var res = _dbContext.BlockChainTransactions.Where(t =>
                        t.FromAddress.Equals(accountAddress) ||
                        t.ToAddress.Equals(accountAddress))
                    .OrderByDescending(t => t.Date).Skip(skipElementsNumber.Value).Take(40);

                return new OkObjectResult(
                    new TransactionsViewModel()
                    {
                        SkipElementsNumber = skipElementsNumber.Value + 40,
                        Transactions = await res.ToListAsync()
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
                var token = await _dbContext.Erc20Tokens.FirstOrDefaultAsync(t =>
                    t.Address.Equals(contractAddress, StringComparison.CurrentCultureIgnoreCase));

                if (token == null)
                    return NotFound("Token not Found");

                token.Quantity = Web3.Convert.FromWei(await _explorer.GetTokenTotalSupply(token.Address),
                    token.DecimalPlaces);

                token.TransactionsCount = _dbContext.CustomEventLogs.Count(l => l.ERC20TokenId == token.Id);
                token.WalletsCount = _dbContext.TokenHolders.Count(h => h.ERC20TokenId == token.Id && h.Quantity != 0);

                return new OkObjectResult(token);
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
                var token = _dbContext.Erc20Tokens.ToList().FirstOrDefault(t =>
                    t.Name?.Equals(contractName, StringComparison.CurrentCultureIgnoreCase) ?? false);

                if (token == null)
                    return NotFound();

                token.Quantity = Web3.Convert.FromWei(await _explorer.GetTokenTotalSupply(token.Address),
                    token.DecimalPlaces);

                token.TransactionsCount = _dbContext.CustomEventLogs.Count(l => l.ERC20TokenId == token.Id);
                token.WalletsCount = _dbContext.TokenHolders.Count(h => h.ERC20TokenId == token.Id && h.Quantity != 0);

                return new OkObjectResult(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSmartContractTransactions(int? skipElementsNumber, string accountAddress)
        {
            try
            {
                skipElementsNumber = skipElementsNumber ?? 0;

                var res = _dbContext.BlockChainTransactions.Where(t =>
                        t.ContractAddress.Equals(accountAddress))
                    .OrderByDescending(t => t.Date).Skip(skipElementsNumber.Value).Take(40);

                return new OkObjectResult(
                    new TransactionsViewModel()
                    {
                        SkipElementsNumber = skipElementsNumber.Value + 40,
                        Transactions = await res.ToListAsync()
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
                var token = _dbContext.Erc20Tokens.ToList().FirstOrDefault(t =>
                    t.Name?.Equals(contractName) ?? false);

                if (token == null)
                    return NotFound();

                var res = _dbContext.BlockChainTransactions.Where(t =>
                        t.ContractAddress.Equals(token.Address, StringComparison.CurrentCultureIgnoreCase))
                    .OrderByDescending(t => t.Date).Take(40);

                return new OkObjectResult(
                    new TransactionsViewModel()
                    {
                        SkipElementsNumber = 40,
                        Transactions = await res.ToListAsync()
                    }
                );
            }
            catch (Exception e)
            {
                return BadRequest(HttpErrorHandler.AddError("Failure", e.Message, ModelState));
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SaveLatestTransactions()
        {
            var status = _dbContext.PageData.FirstOrDefault();
            if (status != null && status.IsTransactionsSaved)
                return Ok();

            var lastKnownBlockNumber = (int) (await _explorer.GetLastAvailableBlockNumber()).Value;
            var tasks = new List<Task<List<BlockChainTransaction>>>();
            for (int i = lastKnownBlockNumber - 5000; i < lastKnownBlockNumber; i += 100)
            {
                var i1 = i;
                var task = Task.Run(() => _explorer.GetLatestTransactions(i1, i1 + 99));
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            var result = new List<BlockChainTransaction>();
            foreach (var task in tasks)
            {
                task.Result.ForEach(t => result.Add(t));
            }

            _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            var tempList = new List<BlockChainTransaction>();
            foreach (var transact in result)
            {
                tempList.Add(transact);
                if (tempList.Count == 100)
                {
                    try
                    {
                        _dbContext.BlockChainTransactions.AddRange(tempList);
                        _dbContext.SaveChanges();
                        tempList.Clear();
                    }
                    catch (Exception e)
                    {
                        tempList.Clear();
                    }
                }
            }

            _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;

            var lastSavedNumber = (int) (_dbContext.BlockChainTransactions
                .Max(w => w.BlockNumber));

            lastKnownBlockNumber = (int) (await _explorer.GetLastAvailableBlockNumber()).Value;
            var newTransacts = _explorer.GetLatestTransactions(lastSavedNumber, lastKnownBlockNumber);

            _dbContext.BlockChainTransactions.AddRange(newTransacts);
            _dbContext.SaveChanges();

            var data = _dbContext.PageData.FirstOrDefault();
            if (data != null)
            {
                data.IsTransactionsSaved = true;
                _dbContext.PageData.Update(data);
            }

            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> StatusSyncTransactions()
        {
            try
            {
                var lastBlockNumber = (int) (await _explorer.GetLastAvailableBlockNumber()).Value;
                var lastCheckedBlockNumber = _dbContext.BlockChainTransactions
                    .Max(w => w.BlockNumber);

                return new OkObjectResult(
                    new StatusSyncTransaction()
                    {
                        CurrentBlock = lastCheckedBlockNumber,
                        LastBlockBl = lastBlockNumber
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(HttpErrorHandler.AddError("Failure", ex.Message, ModelState));
            }
        }

        private IQueryable<TokenHolder> SortHoldersInfo(SortOrder sortOrder, IQueryable<TokenHolder> holders)
        {
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

            return holders;
        }
    }

    public enum SortOrder
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