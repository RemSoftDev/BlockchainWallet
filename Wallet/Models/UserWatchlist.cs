using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wallet.Models
{
    public class UserWatchlist
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public bool IsContract { get; set; }

        [Required]
        public int TokenDecimalPlaces { get; set; }

        public NotificationOptions NotificationOptions { get; set; }
    }
}
