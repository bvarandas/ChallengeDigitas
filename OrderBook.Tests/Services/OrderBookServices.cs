using AutoMapper;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using OrderBook.API.Responses;
using OrderBook.Application;
using OrderBook.Application.Automapper;
using OrderBook.Application.Commands;
using OrderBook.Application.Handlers;
using OrderBook.Application.Interfaces;
using OrderBook.Application.Responses.Books;
using OrderBook.Application.ViewModel;
using OrderBook.Core.Enumerations;
using OrderBook.Core.Repositories;
using OrderBook.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Tests.Services;

public class OrderBookServices
{
    private readonly Mock<IOrderBookRepository> _orderBookRepositoryMock;
    private readonly Mock<IOrderTradeRepository> _orderTradeRepositoryMock;
    private readonly Mock<ILogger<OrderBookService>> _loggerOrderBookServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private BookLevel[] _bids;
    private BookLevel[] _asks;
    
    IMapper _mapper;
    MapperConfiguration _config;
    
    public OrderBookServices()
    {
        _mapperMock = new();
        _orderTradeRepositoryMock = new();
        _orderBookRepositoryMock = new();
        _mediatorMock = new();
        _loggerOrderBookServiceMock = new();
        _config = new MapperConfiguration(cfg => cfg.AddMaps(
            new[] { 
                typeof(DomainToViewModelMappingProfile),
                typeof(ViewModelToDomainMappingProfile)
            }));

        _mapper = _config.CreateMapper();
    }

    [Fact]
    public async Task AddOrderBookCacheAsync_Should_ReturnFalseResult_When_Insert_OrderBookWith_TickerEmpty()
    {
        // Arrange
        var ticker = string.Empty;

        var orderBook = new Application.Responses.Books.OrderBook();
        orderBook.Ticker = ticker;
        orderBook.Timestamp = DateTime.UtcNow;
        orderBook.Asks = null!;
        orderBook.Bids = null!;

        var service = new OrderBookService(_mapperMock.Object, _mediatorMock.Object, _loggerOrderBookServiceMock.Object);
        // Action
        Result<bool> result = await service.AddOrderBookCacheAsync(orderBook);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task AddOrderBookCacheAsync_Should_ReturnFalseResult_When_Insert_OrderBookWith_BidsAndAsksNull()
    {
        // Arrange
        var ticker = "btcusd";

        var orderBook = new Application.Responses.Books.OrderBook();
        orderBook.Ticker = ticker;
        orderBook.Timestamp = DateTime.UtcNow;
        orderBook.Asks = null!;
        orderBook.Bids = null!;

        var service = new OrderBookService(_mapperMock.Object, _mediatorMock.Object, _loggerOrderBookServiceMock.Object);
        // Action
        Result<bool> result = await service.AddOrderBookCacheAsync(orderBook);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    private async void UpdateBidsAsksAsync()
    {
        var bids = new List<BookLevel>();
        var asks = new List<BookLevel>();

        for (int i = 0; i < 10; i++)
        {
            bids.Add(new BookLevel() { Amount = i + 0.1544, Price = i + 0.454, Side = OrderBookSide.Bid, OrderId = 0 });
            asks.Add(new BookLevel() { Amount = i + 0.1544, Price = i + 0.454, Side = OrderBookSide.Ask, OrderId = 0 });
        }
        _bids = bids.ToArray();
        _asks = asks.ToArray();
    }

    [Fact]
    public async Task AddOrderBookCacheAsync_Should_ReturnSuccessResult_When_Insert_OrderBook()
    {
        // Arrange
        var ticker = "btcusd";
        UpdateBidsAsksAsync();
        var orderBook = new Application.Responses.Books.OrderBook();
        orderBook.Ticker = ticker;
        orderBook.Timestamp = DateTime.UtcNow;
        orderBook.Asks = _asks;
        orderBook.Bids = _bids;

        var service = new OrderBookService(_mapperMock.Object, _mediatorMock.Object, _loggerOrderBookServiceMock.Object);
        // Action
        Result<bool> result = await service.AddOrderBookCacheAsync(orderBook);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendOrderTradeAsync_Should_ReturnFailedResult_When_InsertOrderTrade_Hasnt_Quotes()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var orderBookCommand = new InsertOrderTradeCommand(id.ToString(), "btcusd",10, TradeSide.Buy, null!, 0.0, 0.0);
        var service = new OrderBookService(_mapperMock.Object, _mediatorMock.Object, _loggerOrderBookServiceMock.Object);
        // Action
        var result = await service.SendOrderTradeAsync(orderBookCommand);

        // Assert
        Assert.IsType(typeof(OrderTradeViewModel), result);
        Assert.Equal(string.Empty,result.Id);
        Assert.Equal(0.0,result.TotalPriceShaved);
        Assert.Null(result.Quotes);
    }

    [Fact]
    public async Task SendOrderTradeAsync_Should_ReturnFailedResult_When_InsertOrderTrade_Hasnt_Ticker()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var orderBookCommand = new InsertOrderTradeCommand(id.ToString(), "", 10, TradeSide.Buy, null!, 0.0, 0.0);
        var service = new OrderBookService(_mapperMock.Object, _mediatorMock.Object, _loggerOrderBookServiceMock.Object);
        // Action
        var result = await service.SendOrderTradeAsync(orderBookCommand);

        // Assert
        Assert.IsType(typeof(OrderTradeViewModel), result);
        Assert.Equal(string.Empty, result.Id);
        Assert.Equal(0.0, result.TotalPriceShaved);
        Assert.Null(result.Quotes);
    }

    [Fact]
    public async Task SendOrderTradeAsync_Should_ReturnFailedResult_When_InsertOrderTrade_Hasnt_QuanityRequest()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var orderBookCommand = new InsertOrderTradeCommand(id.ToString(), "btcusd", 0, TradeSide.Buy, null!, 0.0, 0.0);
        var service = new OrderBookService(_mapperMock.Object, _mediatorMock.Object, _loggerOrderBookServiceMock.Object);
        // Action
        var result = await service.SendOrderTradeAsync(orderBookCommand);

        // Assert
        Assert.IsType(typeof(OrderTradeViewModel), result);
        Assert.Equal(string.Empty, result.Id);
        Assert.Equal(0.0, result.TotalPriceShaved);
        Assert.Null(result.Quotes);
    }

    [Fact]
    public async Task SendOrderTradeAsync_Should_ReturnFailedResult_When_InsertOrderTrade_Hasnt_QuanityRequest_IsNegative()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var orderBookCommand = new InsertOrderTradeCommand(id.ToString(), "btcusd", -10, TradeSide.Buy, null!, 0.0, 0.0);
        var service = new OrderBookService(_mapperMock.Object, _mediatorMock.Object, _loggerOrderBookServiceMock.Object);
        // Action
        var result = await service.SendOrderTradeAsync(orderBookCommand);

        // Assert
        Assert.IsType(typeof(OrderTradeViewModel), result);
        Assert.Equal(string.Empty, result.Id);
        Assert.Equal(0.0, result.TotalPriceShaved);
        Assert.Null(result.Quotes);
    }

    [Fact]
    public async Task SendOrderTradeAsync_Should_ReturnSuccessResult_When_InsertOrderTrade()
    {
        // Arrange
        UpdateBidsAsksAsync();
        
        var id = ObjectId.GenerateNewId();
        var orderBookCommand = new InsertOrderTradeCommand(id.ToString(), "btcusd", 10, TradeSide.Buy, null!, 0.0, 0.0);
        var service = new OrderBookService(_mapper, _mediatorMock.Object, _loggerOrderBookServiceMock.Object);
        
        // Action
        var orderBook = new Application.Responses.Books.OrderBook();
        orderBook.Ticker = "btcusd";
        orderBook.Timestamp = DateTime.UtcNow;
        orderBook.Asks = _asks;
        orderBook.Bids = _bids;
        
        await service.AddOrderBookCacheAsync(orderBook);

        //_mapperMock.Setup(x => x.Map<IList<OrderBook.Application.Responses.Books.BookLevel>, IList<BookLevelCommand>>(It.IsAny<IList<BookLevel>>())).Returns();

        var result = await service.SendOrderTradeAsync(orderBookCommand);

        // Assert
        Assert.IsType(typeof(OrderTradeViewModel), result);
        Assert.True( result.TotalPriceShaved >=0);
        Assert.NotNull(result.Quotes);
    }

    
    
}
