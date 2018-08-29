using System;

namespace Wallet.Models
{
    public class BlockChainTransaction
    {
        public int Id { get; set; }
        public string TransactionHash { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string What { get; set; }
        public bool IsSuccess { get; set; }
        public string ContractAddress { get; set; }
        
        public decimal DecimalValue { get; set; }
        public int BlockNumber { get; set; }
        public DateTime Date { get; set; }
    }
}