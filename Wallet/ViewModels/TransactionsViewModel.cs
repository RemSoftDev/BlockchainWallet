using System.Collections.Generic;
using Wallet.BlockchainAPI.Model;

namespace Wallet.ViewModels
{
    public class TransactionsViewModel
    {
        public List<CustomTransaction> Transactions { get; set; }

        public int BlockNumber { get; set; }
    }
}