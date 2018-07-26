using System.Threading.Tasks;
using Wallet.ViewModels;

namespace Wallet.Helpers
{
    public interface IParser
    {
        Task<ContractHoldersAndTransactionsModel> GetContractHoldersAndTransactions(string contractAddress);
    }
}