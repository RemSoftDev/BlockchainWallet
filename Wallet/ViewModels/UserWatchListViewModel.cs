using System.ComponentModel.DataAnnotations;
using Wallet.Models;

namespace Wallet.ViewModels
{
    public class UserWatchListViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public bool IsContract { get; set; }

        public NotificationOptions NotificationOptions { get; set; }

        public bool IsNotificated { get; set; }

    }
}