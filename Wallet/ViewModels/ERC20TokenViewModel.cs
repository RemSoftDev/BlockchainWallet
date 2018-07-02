namespace Wallet.ViewModels
{
    public class ERC20TokenViewModel
    {
        public string Address { get; set; }

        public string Symbol { get; set; }
      
        public int DecimalPlaces { get; set; }

        public decimal Balance { get; set; }
    }
}