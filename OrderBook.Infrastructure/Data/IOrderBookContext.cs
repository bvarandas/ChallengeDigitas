using MongoDB.Driver;
using OrderBook.Core.AggregateObjects;
namespace OrderBook.Infrastructure.Data;
public interface IOrderBookContext
{
    IMongoCollection<OrderBookRoot> OrderBooks { get; }
}
