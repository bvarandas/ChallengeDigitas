using OrderBook.Core.Specs;
namespace OrderBook.Core.Repositories;
public interface IOrderBookRepository
{
    Task<IEnumerable<Core.Entities.OrderBook>> GetOrderBooks(OrderBookSpecParams specParams);
    Task<bool> CreateOrderBook(Core.Entities.OrderBook orderBooks);
    Task<bool> UpdateOrderBook(Core.Entities.OrderBook orderBooks);
}
