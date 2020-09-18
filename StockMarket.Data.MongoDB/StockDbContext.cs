using MongoDB.Driver;

namespace StockMarket.Data.MongoDB
{
    public class StockDbContext : IStockDbContext
    {
        private readonly IMongoDatabase database = null;
        private readonly IMongoCollection<StockDocument> stocks = null;

        public StockDbContext(IStockMongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.DatabaseName);
            stocks = database.GetCollection<StockDocument>(settings.StocksCollectionName);
        }

        public IMongoCollection<StockDocument> StockCollection => stocks;
    }
}
