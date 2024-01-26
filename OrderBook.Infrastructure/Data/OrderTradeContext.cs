using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
namespace OrderBook.Infrastructure.Data;
public class OrderTradeContext : IOrderTradeContext
{
    public IMongoCollection<Core.Entities.OrderTrade> OrderTrade { get; }
    public OrderTradeContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

        OrderTrade = database.GetCollection<Core.Entities.OrderTrade>(
            configuration.GetValue<string>("DatabaseSettings:CollectionNameTrade"));
    }
}