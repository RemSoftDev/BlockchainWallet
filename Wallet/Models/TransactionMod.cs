using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.Models
{
    public class TransactionMod
    {    
        public int id { get; set; }

        public bool IsSuccess { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Hash { get; set; }       

        public decimal DecimalValue { get; set; }

        public DateTime Date { get; set; }

        public string What { get; set; }

        public string ContractAddress { get; set; }

        public int BlockNumber { get; set; }

    }
}
