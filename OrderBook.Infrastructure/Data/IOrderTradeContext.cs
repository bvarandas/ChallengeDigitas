using MongoDB.Driver;
namespace OrderBook.Infrastructure.Data;
public  interface IOrderTradeContext
{
    IMongoCollection<Core.AggregateObjects.OrderTrade> OrderTrade { get; }
}