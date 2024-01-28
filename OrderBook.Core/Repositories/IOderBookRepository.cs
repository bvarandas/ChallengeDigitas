using OrderBook.Core.Specs;
namespace OrderBook.Core.Repositories;
public interface IOrderTradeRepository
{
    Task<bool> CreateOrderTradeAsync(Core.AggregateObjects.OrderTrade orderTrade);
}