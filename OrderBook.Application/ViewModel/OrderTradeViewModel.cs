using OrderBook.Application.Responses.Books;
using OrderBook.Core.Enumerations;
using OrderBook.Core.ValuesObject;

namespace OrderBook.Application.ViewModel;
public class OrderTradeViewModel
{
    public string Id { get; set; }=string.Empty;
    public string Ticker { get; set; } = null!;
    public IList<BookLevelViewModel> Quotes { get; set; } = null!;
    public double QuantityRequested { get; set; }
    public TradeSide TradeSide { get; set; }
    public double AmountShaved { get; set; }
    public OrderTradeViewModel(string id, string ticker, IList<BookLevelViewModel> quotes, double quantityRequested, TradeSide tradeSide, double amountShaved)
    {
        Id = id;
        Ticker = ticker;
        Quotes = quotes;
        QuantityRequested = quantityRequested;
        TradeSide = tradeSide;
        AmountShaved = amountShaved;
    }

    public OrderTradeViewModel() { }

}