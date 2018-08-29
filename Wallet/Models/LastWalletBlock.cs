using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.Models
{
    public class LastWalletBlock
    {
        public int Id {get; set; }
        public string Wallet { get; set; }
        public int BlockStart { get; set; }
        public int BlockEnd { get; set; }
        public DateTime Date { get; set; }

    }
}
