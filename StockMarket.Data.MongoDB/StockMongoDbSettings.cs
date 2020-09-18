namespace StockMarket.Data.MongoDB
{
    public class StockMongoDbSettings : IStockMongoDbSettings
    {
        public string StocksCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
