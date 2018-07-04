using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Wallet.BlockchainAPI.Model;
using Wallet.Models;
using Wallet.ViewModels;

namespace Wallet.BlockchainAPI
{
    public interface IBlockchainExplorer
    {
        Task<HexBigInteger> BalanceETH(string account);
        Task<ERC20TokenViewModel> BalanceToken(ERC20Token token, string account);
        Task<List<CustomTransaction>> GetTransactions(string account, int searchInLastBlocksCount);
        Task<HexBigInteger> GetLastAvailableBlockNumber();
    }
}