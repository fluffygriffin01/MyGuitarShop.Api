using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyGuitarShop.Data.MongoDb.Abstract
{
    public abstract class MongoModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; init; } = null!;
    }
}
