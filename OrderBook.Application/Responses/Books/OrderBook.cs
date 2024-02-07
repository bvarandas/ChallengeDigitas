using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using OrderBook.Core.Enumerations;
using ProtoBuf;
using FluentResults;

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

    /// <summary>
    /// Order o book no cache de bids e asks
    /// </summary>
    /// <returns>Task</returns>
    public async Task SortOrderBookCacheAsync()
    {
        Array.Sort(Asks, new AsksComparer());
        Array.Sort(Bids, new BidsComparer());
    }

    /// <summary>
    /// Remove asks e bids com mais de 5 segundos de criação
    /// </summary>
    /// <returns>Tasks</returns>
    public async Task RemoveOldOrderBookCacheAsync()
    {
        var now = DateTime.Now;
        var asksToRemove = Array.FindAll(Asks, x => x.Timestamp < now.AddSeconds(-5));
        var bidsToRemove = Array.FindAll(Bids, x => x.Timestamp < now.AddSeconds(-5));

        Asks = Asks.Except(asksToRemove).ToArray();
        Bids = Bids.Except(bidsToRemove).ToArray();
    }

    /// <summary>
    /// Adiciona novos 
    /// </summary>
    /// <returns></returns>
    public async Task AddOrderBookCacheAsync(OrderBook orderBook)
    {
        var now = DateTime.Now;
        var listAsk = Asks.ToList();
        var listBids = Bids.ToList();

        orderBook.Asks.ToList().ForEach(x =>
        {
            x.Timestamp = now;
            listAsk.Add(x);
        });

        orderBook.Bids.ToList().ForEach(x =>
        {
            x.Timestamp = now;
            listBids.Add(x);
        });

        Asks = listAsk.ToArray();
        Bids = listBids.ToArray();
    }

    public (List<BookLevel>, double) GetQuotesBidAsync(double quantityRequest)
    {
        var result = (new List<BookLevel>(), 0.0);

        double quantityCollected = 0.0;
        Array.ForEach(Bids, bid => {
            if (quantityRequest > quantityCollected && (quantityCollected + bid.Amount) < quantityRequest)
            {
                quantityCollected += bid.Amount;
                result.Item1.Add(bid);
            }
        });
        result.Item2 = quantityCollected;

        return result;
    }

    public (List<BookLevel>, double)  GetQuotesAskAsync(double quantityRequest)
    {
        var result = (new List<BookLevel>(), 0.0);

        double AmountCollected = 0.0;
        Array.ForEach(Asks, ask => {
            if (quantityRequest > AmountCollected && (AmountCollected + ask.Amount) < quantityRequest)
            {
                AmountCollected += ask.Amount;
                result.Item1.Add(ask);
            }
        });

        result.Item2 = AmountCollected;
        
        return result;
    }


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