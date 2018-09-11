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

        public int TokenSentDecimalPlaces { get; set; }

        public double NumberOfTokenOrEtherThatWasSentFrom { get; set; }

        public double NumberOfTokenOrEtherThatWasSentTo { get; set; }

        public string NumberOfTokenOrEtherWasSentName { get; set; }

        public bool WhenTokenOrEtherIsReceived { get; set; }

        public string TokenOrEtherReceivedName { get; set; }

        public bool WhenNumberOfTokenOrEtherWasReceived { get; set; }

        public int TokenReceivedDecimalPlaces { get; set; }

        public double NumberOfTokenOrEtherWasReceived { get; set; }

        public string TokenOrEtherWasReceivedName { get; set; }

        public bool WhenNumberOfContractTokenWasSent { get; set; }

        public double NumberOfContractTokenWasSent { get; set; }

        public bool WhenNumberOfContractWasReceivedByAddress { get; set; }

        public double NumberOfTokenWasReceivedByAddress { get; set; }

        public string AddressThatReceivedNumberOfToken { get; set; }

        public int UserWatchlistId { get; set; }
        public UserWatchlist UserWatchlist { get; set; }
    }
}