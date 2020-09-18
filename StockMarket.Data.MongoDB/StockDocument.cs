using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Data.MongoDB
{
    public class StockDocument
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public decimal PreviousPrice { get; set; }
        public string Exchange { get; set; }
        public bool Favorite { get; set; }
    }
}
