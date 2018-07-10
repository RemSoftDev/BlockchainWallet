using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.Models
{
    public class UserWatchlist
    {
        public int Id { get; set; }
        public string UserId { get; set; } //change it in future on int
        public string Adress { get; set; }
        public int Action { get; set; }
    }
}
