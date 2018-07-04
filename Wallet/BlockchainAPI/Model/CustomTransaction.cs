using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Wallet.BlockchainAPI.Model
{
    public class CustomTransaction : Transaction
    {
        public bool IsSuccess { get; set; }

        public TransactionInput TransferInfo { get; set; }

        public decimal DecimalValue { get; set; }

        public DateTime Date { get; set; }

        public string What { get; set; }

        public string ContractAddress { get; set; }
    }
}
