using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MongoDB.Driver;
using OrderBook.API.Queue;
using OrderBook.Core.AggregateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Tests.Integration;

public class ItemsTests : IClassFixture<MongoDbFixture>
{
    
    private readonly MongoDbFixture _fixture;
    private readonly HttpClient _httpClient;
    private readonly IMongoDatabase _db;
    public readonly IConfiguration _config;
    public readonly IMongoCollection<OrderBookRoot> _orderBookCollection;
    public readonly IMongoCollection<OrderTrade> _orderTradeCollection;

    public readonly IServiceCollection _services = new ServiceCollection();
    public ItemsTests(MongoDbFixture fixture)
    {

        _fixture = fixture;
        
        _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => 
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IMongoClient>();
                    services.TryAddSingleton<IMongoClient>((_) => _fixture.Client);
                    //services.AddHostedService<QueueProducer>();
                });
            });
        _httpClient = appFactory.CreateClient();

        _db = _fixture.Client.GetDatabase(_config.GetValue<string>("DatabaseSettings:DatabaseName"));
        _orderBookCollection = _db.GetCollection<OrderBookRoot>(_config.GetValue<string>("DatabaseSettings:orderbook"));
        _orderTradeCollection = _db.GetCollection<OrderTrade>(_config.GetValue<string>("DatabaseSettings:CollectionNameTrade"));


        _services.AddSingleton<IHostedService, QueueProducer>();
        _services.AddSingleton<IHostedService, QueueProducer>();


    }


}
