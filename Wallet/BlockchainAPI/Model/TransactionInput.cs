using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.BlockchainAPI.Model
{
    public class TransactionInput
    {
        public decimal Value { get; set; }
        public string What { get; set; }
        public string To { get; set; }
        public string ContractAddress { get; set; }
    }
}
