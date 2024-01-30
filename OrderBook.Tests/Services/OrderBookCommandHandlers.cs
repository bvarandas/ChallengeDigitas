using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using OrderBook.Application.Handlers;
using OrderBook.Core.Repositories;
using OrderBook.Application.Commands;
using AutoMapper;
using FluentAssertions;
using ProtoBuf.WellKnownTypes;
using OrderBook.Core.Enumerations;
using MongoDB.Bson;

namespace OrderBook.Tests.Services;
/*
 <ItemGroup>
    <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
        <_Parameter1>OrderBook.Tests</Parameter1>
    </AssemblyAttribute>
 </ItemGroup>
 */
public class OrderBookCommandHandlers
{
    private readonly Mock<IOrderBookRepository> _orderBookRepositoryMock;
    private readonly Mock<IOrderTradeRepository> _orderTradeRepositoryMock;
    private readonly Mock<ILogger<InsertOrderTradeCommandHandler>> _loggerOrderTradeMock;
    private readonly Mock<ILogger<InsertOrderBookCommandHandler>> _loggerOrderBookMock;
    private readonly Mock<IMapper> _mapper;
    private BookLevelCommand[] _bids;
    private BookLevelCommand[] _asks;
    public OrderBookCommandHandlers()
    {
        _orderTradeRepositoryMock = new();
        _orderBookRepositoryMock = new();
        _loggerOrderTradeMock = new();
        _loggerOrderBookMock = new();
        _mapper = new();
        _bids = new BookLevelCommand[] { };
        _asks = new BookLevelCommand[] { };
    }

#region OrderBook

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_When_Insert_OrderBookBidsAsksIsNull()
    {
        var timestamp = DateTime.Now;
        // Arrange
        var command = new InsertOrderBookCommand("btcusd", timestamp, timestamp,null!, null!);

        //_orderBookRepositoryMock.Setup(x=>x.)

        var handler = new InsertOrderBookCommandHandler(_loggerOrderBookMock.Object, _orderBookRepositoryMock.Object, _mapper.Object);
        // Action
        Result<bool> result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_When_Insert_OrderBookBidsAsksIsZero()
    {
        var timestamp = DateTime.Now;
        // Arrange
        var command = new InsertOrderBookCommand("btcusd", timestamp, timestamp, _bids, _asks);

        //_cashFlowRepositoryMock.Setup(x=>x.)

        var handler = new InsertOrderBookCommandHandler(_loggerOrderBookMock.Object, _orderBookRepositoryMock.Object, _mapper.Object);
        // Action
        Result<bool> result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        //result.Error.Should().Be(
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_When_Insert_OrderBorderWithoutTicker()
    {
        var timestamp = DateTime.Now;
        // Arrange
        var command = new InsertOrderBookCommand("", timestamp, timestamp, _bids, _asks);

        //_cashFlowRepositoryMock.Setup(x=>x.)

        var handler = new InsertOrderBookCommandHandler(_loggerOrderBookMock.Object, _orderBookRepositoryMock.Object, _mapper.Object);
        // Action
        Result<bool> result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        //result.Error.Should().Be(
    }

    private async void UpdateBidsAsksAsync()
    {
        var bids = new List<BookLevelCommand>();
        var asks = new List<BookLevelCommand>();

        for (int i =0; i < 10; i++)
        {
            bids.Add(new BookLevelCommand() { Amount = i + 0.1544, Price = i + 0.454, Side = OrderBookSide.Bid, OrderId=0 });
            asks.Add(new BookLevelCommand() { Amount = i + 0.1544, Price = i + 0.454, Side = OrderBookSide.Ask, OrderId=0 });
        }
        _bids = bids.ToArray();
        _asks = asks.ToArray();
    }


    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_When_Insert_OrderBookIsPositive()
    {
        UpdateBidsAsksAsync();

        var timestamp = DateTime.Now;
        // Arrange
        var command = new InsertOrderBookCommand("btcusd", timestamp, timestamp, _bids, _asks);

        _orderBookRepositoryMock.Setup(
            x => x.CreateOrderBookAsync(It.IsAny<Core.AggregateObjects.OrderBookRoot>()))
            .ReturnsAsync(true);

        var handler = new InsertOrderBookCommandHandler(_loggerOrderBookMock.Object, _orderBookRepositoryMock.Object, _mapper.Object);

        // Action
        Result<bool> result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    #endregion

    #region OrderTrade
    IList<BookLevelCommand> _quotes = new List<BookLevelCommand>();
    private async void UpdateQuotes()
    {
        _quotes.Clear();
        for (int i = 0; i < 10; i++)
            _quotes.Add(new BookLevelCommand() { Amount = i + 0.56, Price = i + 0.87, Side = OrderBookSide.Ask });
    }
    [Fact]
    public async Task Handle_Should_ReturnFailureResult_When_Insert_OrderTradeIsNegative()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var command = new InsertOrderTradeCommand(id.ToString(), "btcusd", -100, TradeSide.Sell, null!,45,454  );

        //_orderTradeRepositoryMock.Setup(x =>x.CreateOrderTradeAsync( ))
        //    .ReturnsAsync(false);

        var handler = new InsertOrderTradeCommandHandler(_loggerOrderTradeMock.Object, _mapper.Object, _orderTradeRepositoryMock.Object);
        // Action
        Result<bool> result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_When_Insert_OrderTrade_QuantityRequested_IsZero()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var command = new InsertOrderTradeCommand(id.ToString(), "btcusd", 0, TradeSide.Sell, null!, 45, 454);

        //_cashFlowRepositoryMock.Setup(x => x.AddCashFlow(It.IsAny<CashFlow>()))
        //    .ReturnsAsync(false);

        var handler = new InsertOrderTradeCommandHandler(_loggerOrderTradeMock.Object, _mapper.Object, _orderTradeRepositoryMock.Object);
        // Action
        Result<bool> result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_When_Insert_OrderTradeWithoutTicker()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var command = new InsertOrderTradeCommand(id.ToString(), "", 0, TradeSide.Sell, null!, 45, 454);

        //_cashFlowRepositoryMock.Setup(x => x.AddCashFlow(It.IsAny<CashFlow>()))
        //    .ReturnsAsync(false);

        var handler = new InsertOrderTradeCommandHandler(_loggerOrderTradeMock.Object, _mapper.Object, _orderTradeRepositoryMock.Object);

        // Action
        Result<bool> result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
    }


    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_When_Insert_OrderTradeIsPositivo()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var command = new InsertOrderTradeCommand(id.ToString(), "btcusd", 10, TradeSide.Sell, null!, 45, 454);

        _orderTradeRepositoryMock.Setup(
            x => x.CreateOrderTradeAsync(It.IsAny<OrderBook.Core.AggregateObjects.OrderTrade>()))
            .ReturnsAsync(true);

        var handler = new InsertOrderTradeCommandHandler(_loggerOrderTradeMock.Object, _mapper.Object, _orderTradeRepositoryMock.Object);

        // Action
        Result<bool> result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }


    #endregion
}
