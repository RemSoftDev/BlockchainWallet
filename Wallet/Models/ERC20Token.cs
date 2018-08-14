using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wallet.Models
{
    public class ERC20Token
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public string Symbol { get; set; }

        [JsonProperty(PropertyName = "DecimalPlaces")]
        public int DecimalPlaces { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string WebSiteLink { get; set; }

        public decimal Quantity { get; set; }

        public int TransactionsCount { get; set; }

        public int WalletsCount { get; set; }

        public bool IsSynchronized { get; set; }

        public DateTime? UpdDate { get; set; }

        public ICollection<CustomEventLog> Logs { get; set; }

        public ICollection<TokenHolder> Holders { get; set; }

    }
}