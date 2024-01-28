using OrderBook.Core.AggregateObjects;
using OrderBook.Core.Specs;
namespace OrderBook.Core.Repositories;
public interface IOrderBookRepository
{
    Task<IEnumerable<OrderBookRoot>> GetOrderBooks(OrderBookSpecParams specParams);
    Task<bool> CreateOrderBook(OrderBookRoot orderBooks);
    Task<bool> UpdateOrderBook(OrderBookRoot orderBooks);
}
