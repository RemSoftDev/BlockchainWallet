using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Wallet.Models
{
    public class NotificationOptions
    {
        public int Id { get; set; }

        public bool IsWithoutNotifications { get; set; }

        public bool WhenTokenOrEtherIsSent { get; set; }

        public string TokenOrEtherSentName { get; set; }

        public bool WhenAnythingWasSent { get; set; }

        public bool WhenNumberOfTokenOrEtherWasSent { get; set; }

        public int NumberOfTokenOrEtherThatWasSentFrom { get; set; }

        public int NumberOfTokenOrEtherThatWasSentTo { get; set; }

        public string NumberOfTokenOrEtherWasSentName { get; set; }

        public bool WhenTokenOrEtherIsReceived { get; set; }

        public string TokenOrEtherReceivedName { get; set; }

        public bool WhenNumberOfTokenOrEtherWasReceived { get; set; }

        public int NumberOfTokenOrEtherWasReceived { get; set; }

        public string TokenOrEtherWasReceivedName { get; set; }

        public bool WhenNumberOfContractTokenWasSent { get; set; }

        public int NumberOfContractTokenWasSent { get; set; }

        public bool WhenNumberOfContractWasReceivedByAddress { get; set; }

        public int NumberOfTokenWasReceivedByAddress { get; set; }

        public string AddressThatReceivedNumberOfToken { get; set; }

        public int UserWatchlistId { get; set; }
        public UserWatchlist UserWatchlist { get; set; }
    }
}