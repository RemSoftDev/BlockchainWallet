using System.Collections.Generic;

namespace Wallet.ViewModels
{
    public class WalletInfoViewModel
    {
        public decimal Balance { get; set; }

        public List<ERC20TokenViewModel> Tokens { get; set; }
    }
}