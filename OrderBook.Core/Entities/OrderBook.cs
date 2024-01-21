using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace OrderBook.Core.Entities;

public class OrderBook : BaseEntity
{
    public string Ticker { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DateTime Microtimestamp { get; set; }
    public BookLevel[] Bids { get; set; } = null!;
    public BookLevel[] Asks { get; set; } = null!;
}