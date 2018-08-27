using System.Collections.Generic;

namespace Wallet.Models
{
    public class ChainBlock
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public long TimeStamp { get; set; }
        public ICollection<ChainTransaction> Transactions { get; set; }
    }

    public class ChainTransaction
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Value { get; set; }
        public bool IsSuccess { get; set; }
        public int BlockNumber { get; set; }

        public int ChainBlockId { get; set; }
        public ChainBlock Block { get; set; }
    }
}