using OrderBook.Core.Entities;
using OrderBook.Core.Enumerations;
namespace OrderBook.Core.AggregateObjects;
public class OrderTrade : BaseEntity
{
    public string Ticker { get; set; } = null!;
    public double QuantityRequested { get; set; }
    public TradeSide TradeSide { get; set; }
    public IList<BookLevel> Quotes { get; set; }
    public double AmountShaved { get; set; }
    public double TotalPriceShaved { get; set; }
    public OrderTrade() { }

    public OrderTrade(string ticker, double quantityRequested, TradeSide tradeSide, IList<BookLevel> quotes, double amountShaved, double totalPriceShaved)
    {
        Ticker = ticker;
        QuantityRequested = quantityRequested;
        TradeSide = tradeSide;
        Quotes = quotes;
        AmountShaved = amountShaved;
        TotalPriceShaved = totalPriceShaved;
    }
}