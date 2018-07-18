using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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

        public int Action { get; set; }
    }
}
