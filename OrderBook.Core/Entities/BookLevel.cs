using OrderBook.Core.Enumerations;
namespace OrderBook.Core.Entities;
public class BookLevel
{
    public OrderBookSide Side { get; set; }

    public double Price { get; set; }

    public double Amount { get; set; }

    public long OrderId { get; set; }
}