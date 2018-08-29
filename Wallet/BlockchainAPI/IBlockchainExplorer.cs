using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Wallet.BlockchainAPI.Model;
using Wallet.Models;
using Wallet.ViewModels;

namespace Wallet.BlockchainAPI
{
    public interface IBlockchainExplorer
    {
        Task<HexBigInteger> BalanceETH(string account);
        Task<BigInteger> GetTokenTotalSupply(string contractAddress);
        Task<ERC20TokenViewModel> BalanceToken(ERC20Token token, string account);
        Task<List<CustomTransaction>> GetTransactions(string account, int searchInLastBlocksCount);
        Task<List<CustomTransaction>> GetSmartContractTransactions(string account, int searchInLastBlocksCount);
        Task<HexBigInteger> GetLastAvailableBlockNumber();
        Task<string> GetCode(string address);
        Task<BlockWithTransactions> GetBlockByNumber(int blockNumber);
        Task<List<CustomEventLog>> GetFullEventLogs(ERC20Token contract, int lastBlockNumber, int blocknum =1);
        Task<BigInteger> GetTokenHolderBalance(string holderAddress, string contractAddress);
        void SaveNumWalletBlockToTemp(int start, int end, string wallet);
    }
}