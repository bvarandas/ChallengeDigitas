using OrderBook.Core.Enumerations;
namespace OrderBook.Application.ViewModel;
public class OrderTradeModelView
{
    public string Ticker { get; set; }=string.Empty;
    public double AmountRequested { get; set; }
    public TradeSide TradeSide { get; set; }
    public double AmountShaved { get; set; }
    public double TotalAmountPrice { get; set; }
}