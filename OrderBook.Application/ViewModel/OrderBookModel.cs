using OrderBook.Core.Enumerations;
namespace OrderBook.Application.ViewModel;
public class OrderBookModel  
{
    public string Ticker { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public DateTime Microtimestamp { get; set; }

    public BookLevel[] Bids { get; set; } = null!;

    public BookLevel[] Asks { get; set; } = null!;
}

public class BookLevel
{
    public OrderBookSide Side { get; set; }

    public double Price { get; set; }

    public double Amount { get; set; }

    public long OrderId { get; set; }
}
