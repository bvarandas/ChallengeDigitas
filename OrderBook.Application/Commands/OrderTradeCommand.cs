using OrderBook.Core.Enumerations;
namespace OrderBook.Application.Commands;
public class OrderTradeCommand
{
    public string Ticker { get; set; } = string.Empty;
    public double AmountRequested { get; set; }
    public TradeSide TradeSide { get; set; }
}