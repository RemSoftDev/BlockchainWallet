using System.ComponentModel.DataAnnotations;

namespace Wallet.ViewModels
{
    public class ConfirmEmailViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Code { get; set; }
    }
}