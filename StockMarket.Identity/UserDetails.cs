using System.ComponentModel.DataAnnotations;

namespace StockMarket.Identity
{
    public class UserDetails
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; } = "";
    }
}
