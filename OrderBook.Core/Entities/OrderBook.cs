using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrderBook.Core.Enumerations;

namespace OrderBook.Core.Entities;

public class OrderBook : BaseEntity
{
    public string Ticker { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DateTime Microtimestamp { get; set; }
    public BookLevel[] Bids { get; set; } = null!;
    public BookLevel[] Asks { get; set; } = null!;

    public OrderBook(string ticker, DateTime timestamp, DateTime microtimestamp, BookLevel[] bids, BookLevel[] asks)
    {
        Ticker = ticker;
        Timestamp = timestamp;
        Microtimestamp = microtimestamp;
        Bids = bids;
        Asks = asks;
    }

    public OrderBook() { }
}

public class BookLevel
{
    public OrderBookSide Side { get; set; }

    public double Price { get; set; }

    public double Amount { get; set; }

    public long OrderId { get; set; }
}

public class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
}