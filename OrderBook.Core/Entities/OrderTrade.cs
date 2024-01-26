using OrderBook.Core.Enumerations;
using OrderBook.Core.ValuesObject;
namespace OrderBook.Core.Entities;
public class OrderTrade : BaseEntity
{
    public Ticker Ticker { get; set; } = null!;
    public double QuantityRequested { get; set; }
    public TradeSide TradeSide { get; set; }
    public IList<BookLevel> Quotes { get; set; }
    public double AmountShaved { get; set; }
    public OrderTrade() { }

    public OrderTrade(Ticker ticker, double quantityRequested, TradeSide tradeSide, IList<BookLevel> quotes, double amountShaved)
    {
        Ticker = ticker;
        QuantityRequested = quantityRequested;
        TradeSide = tradeSide;
        Quotes = quotes;
        AmountShaved = amountShaved;
    }
}