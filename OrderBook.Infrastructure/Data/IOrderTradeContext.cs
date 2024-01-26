using MongoDB.Driver;
namespace OrderBook.Infrastructure.Data;
public  interface IOrderTradeContext
{
    IMongoCollection<Core.Entities.OrderTrade> OrderTrade { get; }
}