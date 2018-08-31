namespace Wallet.Models
{
    public class PageData
    {
        public int PageDataId { get; set; }

        public string ElementName { get; set; }
        
        public string ElementData { get; set; }

        public bool IsTransactionsSaved { get; set; }
    }
}