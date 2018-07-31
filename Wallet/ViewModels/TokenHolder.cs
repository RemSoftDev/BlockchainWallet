using System;
using System.ComponentModel.DataAnnotations;

namespace Wallet.Models
{
    public class TokenHolder
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public decimal Quantity { get; set; }

        public decimal TokensSent { get; set; }

        public decimal TokensReceived { get; set; }

        public int GeneralTransactionsNumber { get; set; }

        public int SentTransactionsNumber { get; set; }

        public int ReceivedTransactionsNumber { get; set; }

        public DateTime DateTime { get; set; }

    }
}