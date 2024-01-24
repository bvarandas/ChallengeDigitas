using System.Runtime.Serialization;
using System.Runtime.Serialization;

namespace OrderBook.Core.Enumerations;

/// <summary>
/// Websockets channels
/// </summary>
public enum Channel
{
    /// <summary>
    /// Heartbeat
    /// </summary>
    Heartbeat,

    /// <summary>
    /// Ticker
    /// </summary>
    Ticker,

    /// <summary>
    /// Orders
    /// </summary>
    Orders,

    /// <summary>
    /// OrderBook
    /// </summary>
    OrderBook,

    /// <summary>
    /// OrderBookDetail
    /// </summary>
    OrderBookDetail,

    /// <summary>
    /// OrderBookDiff
    /// </summary>
    OrderBookDiff
}


public enum MessageType
{
    // Do not rename, used in requests
    [DataMember(Name = "bts:subscribe")] Subscribe,
    [DataMember(Name = "bts:unsubscribe")] Unsubscribe,

    // Can be renamed, only for responses
    Error,
    Info,
    Trade,
    OrderBook,
    Wallet,
    Order,
    Position,
    Quote,
    Instrument,
    Margin,
    Execution,
    Funding,
    OrderBookDiff,
    OrderBookDetail,
    Snapshot
}


public enum OrderBookSide
{
    Undefined,
    Bid,
    Ask
}

public enum TradeSide
{
    Undefined,
    Buy,
    Sell
}