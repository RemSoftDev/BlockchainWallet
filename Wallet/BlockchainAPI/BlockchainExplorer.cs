using Nethereum.Hex.HexConvertors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Wallet.BlockchainAPI.Model;
using Wallet.Helpers;
using Wallet.Models;
using Wallet.ViewModels;

namespace Wallet.BlockchainAPI
{
    public class BlockchainExplorer : IBlockchainExplorer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Web3 web3;

        public BlockchainExplorer(IServiceScopeFactory scopeFactory)
        {
            web3 = new Web3();
            _scopeFactory = scopeFactory;
        }

        public Task<string> GetCode(string address)
        {
            return web3.Eth.GetCode.SendRequestAsync(address);
        }

        public async Task<HexBigInteger> GetLastAvailableBlockNumber()
        {
            return await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
        }

        public Task<BlockWithTransactions> GetBlockByNumber(int blockNumber)
        {
            return web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(blockNumber));
        }

        private async Task<List<CustomTransaction>> GetTransactionByAddress(string accountAddress,
            List<Transaction> transactions, BigInteger timestamp)
        {
            var result = new List<CustomTransaction>();

            foreach (var t in transactions)
            {
                if (t.Input.Equals(Constants.Strings.TransactionType.Usual,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    if (t.From.Equals(accountAddress, StringComparison.CurrentCultureIgnoreCase) ||
                        t.To.Equals(accountAddress, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var status =
                            await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(t.TransactionHash);
                        bool isSuccess = true;
                        if (status != null)
                        {
                            if (status.Status.Value == 0)
                            {
                                isSuccess = false;
                            }
                        }

                        result.Add(new CustomTransaction()
                        {
                            TransactionHash = t.TransactionHash,
                            From = t.From,
                            To = t.To,
                            What = "ETH",
                            IsSuccess = isSuccess,
                            DecimalValue = Web3.Convert.FromWei(t.Value.Value, 18),
                            Date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(
                                (long)(timestamp))
                        });
                    }
                } //get token transfer
                else if (t.Input.StartsWith(Constants.Strings.TransactionType.Transfer))
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
                        var status =
                            await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(t.TransactionHash);
                        bool isSuccess = true;
                        if (status != null)
                        {
                            if (status.Status.Value == 0)
                            {
                                isSuccess = false;
                            }
                        }

                        var decodedInput = InputDecoder.DecodeTransferInput(t.Input);

                        if (t.From.Equals(accountAddress, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var token = dbContext.Erc20Tokens.FirstOrDefault(tok =>
                                tok.Address.Equals(t.To, StringComparison.CurrentCultureIgnoreCase));

                            result.Add(new CustomTransaction()
                            {
                                TransactionHash = t.TransactionHash,
                                From = t.From,
                                To = decodedInput.To,
                                What = token?.Symbol ?? "Unknown",
                                IsSuccess = isSuccess,
                                ContractAddress = t.To,
                                DecimalValue = Web3.Convert.FromWei(decodedInput.Value, token?.DecimalPlaces ?? 18),
                                Date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(
                                    (long)(timestamp))
                            });
                        }
                        else if (decodedInput.To.Equals(accountAddress, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var token = dbContext.Erc20Tokens.FirstOrDefault(tok =>
                                tok.Address.Equals(t.To, StringComparison.CurrentCultureIgnoreCase));

                            result.Add(new CustomTransaction()
                            {
                                TransactionHash = t.TransactionHash,
                                From = t.From,
                                To = decodedInput.To,
                                What = token?.Symbol ?? "Unknown",
                                IsSuccess = isSuccess,
                                ContractAddress = t.To,
                                DecimalValue = Web3.Convert.FromWei(decodedInput.Value, token?.DecimalPlaces ?? 18),
                                Date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(
                                    (long)(timestamp))
                            });
                        }
                    }
                }
            }

            return result;
        }

        public async Task<List<CustomTransaction>> GetTransactions(string account, int blockNumber)
        {
            try
            {
                var block = (await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(
                    new HexBigInteger(blockNumber)));

                return await GetTransactionByAddress(account, block.Transactions.ToList(), block.Timestamp.Value);
            }
            catch (Exception e)
            {
                return new List<CustomTransaction>();
            }
        }

        /// <summary>
        /// get current balance eth
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task<HexBigInteger> BalanceETH(string account)
        {
            return web3.Eth.GetBalance.SendRequestAsync(account);
        }

        /// <summary>
        /// get token erc20
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        /// 
        public async Task<ERC20TokenViewModel> BalanceToken(ERC20Token token, string account)
        {
            try
            {
                var cont = web3.Eth.GetContract(Constants.Strings.ABI.Abi, token.Address);
                var eth = cont.GetFunction("balanceOf");
                var balance = await eth.CallAsync<BigInteger>(account);
                return new ERC20TokenViewModel()
                {
                    Address = token.Address,
                    Balance = Web3.Convert.FromWei(balance, token.DecimalPlaces),
                    DecimalPlaces = token.DecimalPlaces,
                    Symbol = token.Symbol
                };
            }
            catch (Exception e)
            {
                return new ERC20TokenViewModel()
                {
                    Address = token.Address,
                    Balance = 0,
                    DecimalPlaces = token.DecimalPlaces,
                    Symbol = token.Symbol
                };
            }
        }
    }
}