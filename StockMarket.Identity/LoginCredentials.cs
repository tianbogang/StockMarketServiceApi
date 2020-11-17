using System.ComponentModel.DataAnnotations;

namespace StockMarket.Identity
{
    public record LoginCredentials(
        [Required] string Username,
        [Required] string Password
    );
}
