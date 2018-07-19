using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Wallet.Models
{
    public class NotificationOptions
    {
        public int Id { get; set; }

        public bool IsWithoutNotifications { get; set; }

        public bool WhenTokenIsSent { get; set; }

        public string TokenSentName { get; set; }

        public bool WhenAnythingWasSent { get; set; }

        public bool WhenNumberOfTokenWasSent { get; set; }

        public int NumberOfTokenThatWasSent { get; set; }

        public string NumberOfTokenWasSentName { get; set; }

        public bool WhenTokenIsReceived { get; set; }

        public string TokenReceivedName { get; set; }

        public bool WhenNumberOfTokenWasReceived { get; set; }

        public int NumberOfTokenWasReceived { get; set; }

        public string TokenWasReceivedName { get; set; }

        public bool WhenNumberOfContractTokenWasSent { get; set; }

        public int NumberOfContractTokenWasSent { get; set; }

        public bool WhenNumberOfContractWasReceivedByAddress { get; set; }

        public int NumberOfTokenWasReceivedByAddress { get; set; }

        public string AddressThatReceivedNumberOfToken { get; set; }

        public int UserWatchlistId { get; set; }
        public UserWatchlist UserWatchlist { get; set; }
    }
}