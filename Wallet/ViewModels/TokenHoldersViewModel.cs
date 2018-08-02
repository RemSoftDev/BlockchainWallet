using System.Collections.Generic;
using Wallet.Models;

namespace Wallet.ViewModels
{
    public class TokenHoldersViewModel
    {
        public List<TokenHolder> HoldersInfo { get; set; }

        public int SkipElementsCount { get; set; }
    }
}