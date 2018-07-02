using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;

namespace Wallet.BlockchainAPI
{
    public interface IBlockchainExplorer
    {
        Task<HexBigInteger> BalanceETH(string account);
    }
}