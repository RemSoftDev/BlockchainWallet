using System.ComponentModel.DataAnnotations;

namespace Wallet.ViewModels
{
    public class UserWatchListViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public bool IsContract { get; set; }

    }
}