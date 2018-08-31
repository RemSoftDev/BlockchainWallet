using System;

namespace Wallet.Models
{
    public class BlockChainTransaction
    {
        public int Id { get; set; }
        public string TransactionHash { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string What { get; set; }
        public bool IsSuccess { get; set; }
        public string ContractAddress { get; set; }
        
        public double DecimalValue { get; set; }
        public int BlockNumber { get; set; }
        public DateTime Date { get; set; }
    }
}