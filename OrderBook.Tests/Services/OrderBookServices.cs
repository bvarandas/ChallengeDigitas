using AutoMapper;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using OrderBook.API.Responses;
using OrderBook.Application;
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
    //private readonly Mock<IOrderBookService> _orderBookServiceMock;
    private readonly Mock<IOrderBookRepository> _orderBookRepositoryMock;
    private readonly Mock<IOrderTradeRepository> _orderTradeRepositoryMock;
    //private readonly Mock<ILogger<OrderTradeRepository>> _loggerOrderTradeMock;
    //private readonly Mock<ILogger<OrderBookRepository>> _loggerOrderBookMock;
    private readonly Mock<ILogger<OrderBookService>> _loggerOrderBookServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private BookLevel[] _bids;
    private BookLevel[] _asks;
    public OrderBookServices()
    {
        _mapperMock = new();
        _orderTradeRepositoryMock = new();
        _orderBookRepositoryMock = new();
        //_loggerOrderTradeMock = new();
        //_loggerOrderBookMock = new();
        _mediatorMock = new();
        _loggerOrderBookServiceMock = new();
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
        var service = new OrderBookService(_mapperMock.Object, _mediatorMock.Object, _loggerOrderBookServiceMock.Object);
        
        // Action
        var orderBook = new Application.Responses.Books.OrderBook();
        orderBook.Ticker = "btcusd";
        orderBook.Timestamp = DateTime.UtcNow;
        orderBook.Asks = _asks;
        orderBook.Bids = _bids;
        
        await service.AddOrderBookCacheAsync(orderBook);

        var result = await service.SendOrderTradeAsync(orderBookCommand);

        // Assert
        Assert.IsType(typeof(OrderTradeViewModel), result);
        Assert.Equal(id.ToString(), result.Id);
        Assert.True( result.TotalPriceShaved >=0);
        Assert.NotNull(result.Quotes);
    }


    [Fact]
    public async Task SortOrderBookCacheAsync_Should_ReturnSuccessResult()
    {
        // Arrange
        UpdateBidsAsksAsync();
        var id = ObjectId.GenerateNewId();
        var orderBookCommand = new InsertOrderTradeCommand(id.ToString(), "btcusd", 10, TradeSide.Buy, null!, 0.0, 0.0);
        var service = new OrderBookService(_mapperMock.Object, _mediatorMock.Object, _loggerOrderBookServiceMock.Object);
        
        // Action
        var orderBook = new Application.Responses.Books.OrderBook();
        orderBook.Ticker = "btcusd";
        orderBook.Timestamp = DateTime.UtcNow;
        orderBook.Asks = _asks;
        orderBook.Bids = _bids;

        await service.AddOrderBookCacheAsync(orderBook);

        var result = await service.SendOrderTradeAsync(orderBookCommand);

        // Assert
        Assert.IsType(typeof(OrderTradeViewModel), result);
        Assert.Equal(id.ToString(), result.Id);
        Assert.True(result.TotalPriceShaved >= 0);
        Assert.NotNull(result.Quotes);
    }

    [Fact]
    public async Task RemoveOldOrderBookCacheAsync_Should_ReturnSuccessResult()
    {
        // Arrange
        UpdateBidsAsksAsync();
        var id = ObjectId.GenerateNewId();
        var orderBookCommand = new InsertOrderTradeCommand(id.ToString(), "btcusd", 10, TradeSide.Buy, null!, 0.0, 0.0);
        var service = new OrderBookService(_mapperMock.Object, _mediatorMock.Object, _loggerOrderBookServiceMock.Object);
        // Action
        var orderBook = new Application.Responses.Books.OrderBook();
        orderBook.Ticker = "btcusd";
        orderBook.Timestamp = DateTime.UtcNow;
        orderBook.Asks = _asks;
        orderBook.Bids = _bids;

        await service.AddOrderBookCacheAsync(orderBook);

        var result = await service.RemoveOldOrderBookCacheAsync();//.SendOrderTradeAsync(orderBookCommand);

        // Assert
        Assert.IsType(typeof(OrderTradeViewModel), result);
        Assert.Equal(id.ToString(), result.Id);
        Assert.True(result.TotalPriceShaved >= 0);
        Assert.NotNull(result.Quotes);
    }
}
