using OrderBook.Core.Enumerations;
using OrderBook.Core.ValuesObject;
namespace OrderBook.Application.Commands;
public class OrderTradeCommand
{
    public Ticker Ticker { get; set; } = null!;
    public double QuantityRequested { get; set; }
    public TradeSide TradeSide { get; set; }

    public OrderTradeCommand() { }

    public OrderTradeCommand(Ticker ticker, double quantityRequested, TradeSide tradeSide)
    {
        Ticker = ticker;
        QuantityRequested = quantityRequested;
        TradeSide = tradeSide;
    }
}