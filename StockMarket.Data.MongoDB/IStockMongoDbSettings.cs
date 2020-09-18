namespace StockMarket.Data.MongoDB
{
    public interface IStockMongoDbSettings
    {
        string StocksCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
