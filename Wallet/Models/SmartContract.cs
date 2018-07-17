using System;
using System.ComponentModel.DataAnnotations;

namespace Wallet.Models
{
    public class SmartContract
    {
        public int SmartContractId { get; set; }

        [Required]
        public string Address { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public string WebSiteLink { get; set; }

        public int Quantity { get; set; }

        public int TransactionsCount { get; set; }

        public int WalletsCount { get; set; }

    }
}