using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MongoDB.Driver;
using Moq;
using OrderBook.API.Queue;
using OrderBook.Application.Commands;
using OrderBook.Application.Handlers;
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

    private readonly Mock<ILogger<InsertOrderTradeCommandHandler>> _loggerOrderTradeMock;
    private readonly Mock<ILogger<InsertOrderBookCommandHandler>> _loggerOrderBookMock;

    public readonly IServiceCollection _services = new ServiceCollection();
    public ItemsTests(MongoDbFixture fixture)
    {
        _loggerOrderTradeMock = new();
        _loggerOrderBookMock = new();

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
                });
            });
        _httpClient = appFactory.CreateClient();

        _db = _fixture.Client.GetDatabase(_config.GetValue<string>("DatabaseSettings:DatabaseName"));
        _orderBookCollection = _db.GetCollection<OrderBookRoot>(_config.GetValue<string>("DatabaseSettings:orderbook"));
        _orderTradeCollection = _db.GetCollection<OrderTrade>(_config.GetValue<string>("DatabaseSettings:CollectionNameTrade"));
        
        _services.AddSingleton<IHostedService, QueueProducer>();
    }

    //[Fact]
    //public async Task Post_Handle_Should_ReturnFailureResult_When_Insert_OrderBookBidsAsksIsNull()
    //{
    //    var timestamp = DateTime.Now;
    //    // Arrange
    //    var command = new InsertOrderBookCommand("btcusd", timestamp, timestamp, null!, null!);

    //    //_orderBookRepositoryMock.Setup(x=>x.)

    //    var handler = new InsertOrderBookCommandHandler(_loggerOrderBookMock.Object,_db. _orderBookRepositoryMock.Object, _mapper.Object);
    //    // Action
    //    Result<bool> result = await handler.Handle(command, default);

    //    // Assert
    //    result.IsFailed.Should().BeTrue();
    //}


}
