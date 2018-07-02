using Newtonsoft.Json;

namespace Wallet.Models
{
    public class ERC20Token
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public string Symbol { get; set; }

        [JsonProperty(PropertyName = "decimal")]
        public int DecimalPlaces { get; set; }

        public string Type { get; set; }
    }
}