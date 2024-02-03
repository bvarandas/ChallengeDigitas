using Microsoft.Extensions.Configuration;
using Mongo2Go;
using MongoDB.Driver;
using OrderBook.Infrastructure.Data;

namespace OrderBook.Tests.Integration;
public class DbFixture : IDisposable
{
    public OrderBookContext DbOrderBookContext { get; }
    public OrderTradeContext DbOrderTradeContext { get; }
    public readonly IConfiguration _config;
    public DbFixture()
    {
        _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
                
        this.DbOrderBookContext = new OrderBookContext(_config);
        this.DbOrderTradeContext = new OrderTradeContext(_config);
    }

    public void Dispose()
    {
        var client = new MongoClient(_config.GetValue<string>("DatabaseSettings:ConnectionString"));
        client.DropDatabase(_config.GetValue<string>("DatabaseSettings:DatabaseName"));
    }
}

public class MongoDbFixture : IDisposable
{
    public MongoDbRunner Runner { get; private set; }
    public MongoClient Client { get; private set; }

    public MongoDbFixture()
    {
        Runner = MongoDbRunner.Start();
        Client = new MongoClient(Runner.ConnectionString);
    }

    public void Dispose()
    {
        Runner.Dispose();
    }
}