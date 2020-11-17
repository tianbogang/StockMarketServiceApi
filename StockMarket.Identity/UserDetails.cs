using System.ComponentModel.DataAnnotations;

namespace StockMarket.Identity
{
    public record UserDetails
    (
        [Required] string Username,
        [Required] string Password,
        [Required] string Email
    );
}
