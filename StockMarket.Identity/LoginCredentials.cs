using System.ComponentModel.DataAnnotations;

namespace StockMarket.Identity
{
    public class LoginCredentials
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
