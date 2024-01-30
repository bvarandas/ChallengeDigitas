using OrderBook.Core.AggregateObjects;
using OrderBook.Core.Specs;
namespace OrderBook.Core.Repositories;
public interface IOrderBookRepository
{
    Task<IEnumerable<OrderBookRoot>> GetOrderBooksAsync(OrderBookSpecParams specParams);
    Task<bool> CreateOrderBookAsync(OrderBookRoot orderBooks);
    Task<bool> UpdateOrderBookAsync(OrderBookRoot orderBooks);
}
