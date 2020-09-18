namespace StockMarket.Core.Models
{
    public class Stock
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public decimal PreviousPrice { get; set; }
        public string Exchange { get; set; }
        public bool Favorite { get; set; }
    }
}
