namespace StockMarket.Core.Models
{
    public record Stock(string Name, string Code, decimal Price, decimal PreviousPrice, string Exchange, bool Favorite);

    /*
    public class Stock
    {
        public string Name { get; init; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public decimal PreviousPrice { get; set; }
        public string Exchange { get; set; }
        public bool Favorite { get; set; }
    }
    // */
}
