using OrderBook.Core.Entities;
using OrderBook.Core.Enumerations;

namespace OrderBook.Core.AggregateObjects;

public class OrderBookRoot : BaseEntity
{
    public string Ticker { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime Microtimestamp { get; set; }
    public BookLevel[] Bids { get; set; } = null!;
    public BookLevel[] Asks { get; set; } = null!;

    public OrderBookRoot(string ticker, DateTime timestamp, DateTime microtimestamp, BookLevel[] bids, BookLevel[] asks)
    {
        Ticker = ticker;
        Timestamp = timestamp;
        Microtimestamp = microtimestamp;
        Bids = bids;
        Asks = asks;
    }

    public OrderBookRoot() { }
}