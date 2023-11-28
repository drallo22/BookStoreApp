using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class ChangePasswordViewModel
    {
        public string Username { get; set; }
        public string NewPassword { get; set; }
    }
}
