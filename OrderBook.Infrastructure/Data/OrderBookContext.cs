using Amazon.Runtime.Internal;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace OrderBook.Infrastructure.Data;

public class OrderBookContext : IOrderBookContext
{
    public IMongoCollection<Core.AggregateObjects.OrderBookRoot> OrderBooks { get; set; }

    public OrderBookContext (IConfiguration configuration )
    {
        var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

        OrderBooks = database.GetCollection<Core.AggregateObjects.OrderBookRoot>(
            configuration.GetValue<string>("DatabaseSettings:CollectionName"));
    }
}
