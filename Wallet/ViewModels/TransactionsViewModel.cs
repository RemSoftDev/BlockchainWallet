using System.Collections.Generic;
using Wallet.Models;

namespace Wallet.ViewModels
{
    public class TransactionsViewModel
    {
        public List<BlockChainTransaction> Transactions { get; set; }

        public int SkipElementsNumber { get; set; }
    }
}