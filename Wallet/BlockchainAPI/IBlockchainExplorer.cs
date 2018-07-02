using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Wallet.Models;
using Wallet.ViewModels;

namespace Wallet.BlockchainAPI
{
    public interface IBlockchainExplorer
    {
        Task<HexBigInteger> BalanceETH(string account);
        Task<ERC20TokenViewModel> BalanceToken(ERC20Token token, string account);
    }
}