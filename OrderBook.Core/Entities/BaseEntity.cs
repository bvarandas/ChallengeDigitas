using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderBook.Core.Entities;
public class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
}
