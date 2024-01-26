using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using OrderBook.Core.Enumerations;
using ProtoBuf;

namespace OrderBook.Application.Responses.Books;

[ProtoContract]
public class OrderBook : BaseEntity
{
    public OrderBook()
    {
    }

    /// <summary>
    /// Ticker - crypto with currency
    /// </summary>
    [ProtoMember(1)]
    public string Ticker { get; set; }=string.Empty;

    /// <summary>
    /// Timestamp - datetime only seconds
    /// </summary>
    [ProtoMember(2)]
    [JsonConverter(typeof(UnixDateTimeSecondsConverter))]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Microtimestamp - datetime with milliseconds
    /// </summary>
    [ProtoMember(3)]
    [JsonConverter(typeof(UnixDateTimeMillisecondsConverter))]
    public DateTime Microtimestamp { get; set; }

    /// <summary>
    /// Order book bid levels
    /// </summary>
    [JsonConverter(typeof(OrderBookLevelConverter), OrderBookSide.Bid)]
    [ProtoMember(4)]
    public BookLevel[] Bids { get; set; }

    /// <summary>
    /// Order book ask levels
    /// </summary>
    [ProtoMember(5)]
    [JsonConverter(typeof(OrderBookLevelConverter), OrderBookSide.Ask)]
    public BookLevel[] Asks { get; set; }
}

[ProtoContract]
public class BaseEntity
{
    [ProtoMember(6)]
    public string Id { get; set; } = string.Empty;
    public BaseEntity() { }
}

[ProtoContract]
public class BookLevel
{
    public BookLevel()
    {
        
    }

    [ProtoMember(1)]
    public OrderBookSide Side { get; set; }

    [ProtoMember(2)]
    public double Price { get; set; }

    [ProtoMember(3)]
    public double Amount { get; set; }

    [ProtoMember(4)]
    public long OrderId { get; set; }

    [ProtoMember(5)]
    public DateTime Timestamp { get; set; }
}