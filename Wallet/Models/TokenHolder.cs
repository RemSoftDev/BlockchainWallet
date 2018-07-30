using System.ComponentModel.DataAnnotations;

namespace Wallet.Models
{
    public class TokenHolder
    {
        public int Id { get; set; }

        [Required]
        public string Address { get; set; }

        public int Quantity { get; set; }

        public int TokensSent { get; set; }

        public int TokensReceived { get; set; }

        public int GeneralTransactionsNumber { get; set; }

        public int SentTransactionsNumber { get; set; }

        public int ReceivedTransactionsNumber { get; set; }

        public int ERC20TokenId { get; set; }
        public ERC20Token Token { get; set; }
    }
}