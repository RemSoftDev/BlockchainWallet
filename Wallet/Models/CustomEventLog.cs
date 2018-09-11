using System;

namespace Wallet.Models
{
    public class CustomEventLog
    {
        public int Id { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public decimal AmountOfToken { get; set; }

        public DateTime WhenDateTime  { get; set; }

        public int BlockNumber { get; set; }

        public int ERC20TokenId { get; set; }
        public ERC20Token Token { get; set; }
    }
}