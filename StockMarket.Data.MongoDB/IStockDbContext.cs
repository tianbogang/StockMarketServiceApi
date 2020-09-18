using MongoDB.Driver;

namespace StockMarket.Data.MongoDB
{
    public interface IStockDbContext
    {
        IMongoCollection<StockDocument> StockCollection { get; }
    }
}
