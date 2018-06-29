using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Wallet.BlockchainAPI.Model
{
    class CustomTransaction : Transaction
    {
        public bool IsSuccess { get; set; }

        public TransactionInput TransferInfo { get; set; }

        public decimal DecimalValue { get; set; }

        public TimeSpan Date { get; set; }
    }
}
