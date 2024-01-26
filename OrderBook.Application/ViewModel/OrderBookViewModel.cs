using OrderBook.Core.Enumerations;
namespace OrderBook.Application.ViewModel;
public class OrderBookViewModel  
{
    public string Ticker { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DateTime Microtimestamp { get; set; }
    public BookLevelViewModel[] Bids { get; set; } = null!;
    public BookLevelViewModel[] Asks { get; set; } = null!;
    public OrderBookViewModel(string ticker, DateTime timestamp, DateTime microtimestamp, BookLevelViewModel[] bids, BookLevelViewModel[] asks)
    {
        Ticker = ticker;
        Timestamp = timestamp;
        Microtimestamp = microtimestamp;
        Bids = bids;
        Asks = asks;
    }
}
public class BookLevelViewModel
{
    public OrderBookSide Side { get; set; }
    public double Price { get; set; }
    public double Amount { get; set; }
    public long OrderId { get; set; }
}

public class OrderBookDataViewModel
{
    public string Ticker { get; set; }=string.Empty;
    public double MaxPrice { get; set; }
    public double MinPrice { get; set; }
    public double AveragePrice { get; set; }
    public double AveragePriceLast5Seconds { get; set; }
    public double AverageAmountQuantity { get; set; }
}