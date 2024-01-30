using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using OrderBook.Application.Handlers;
using OrderBook.Core.Repositories;
using OrderBook.Application.Commands;
using AutoMapper;
using FluentAssertions;
using ProtoBuf.WellKnownTypes;
using System.Reactive;
using OrderBook.Core.Enumerations;

namespace OrderBook.Tests.Services;

public class OrderBookCommandHandlers
{
    private readonly Mock<IOrderBookRepository> _orderBookRepositoryMock;
    private readonly Mock<ILogger<InsertOrderTradeCommandHandler>> _loggerUpdateTradeMock;
    private readonly Mock<ILogger<InsertOrderBookCommandHandler>> _loggerInsertBookMock;
    private readonly Mock<IMapper> _mapper;
    private BookLevelCommand[] _bids;
    private BookLevelCommand[] _asks;
    public OrderBookCommandHandlers()
    {
        _orderBookRepositoryMock = new();
        _loggerUpdateTradeMock = new();
        _loggerInsertBookMock = new();
        _mapper = new();
        _bids = new BookLevelCommand[] { };
        _asks = new BookLevelCommand[] { };
    }


#region Insert

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_When_Insert_OrderBookBidsAsksIsNull()
    {
        var timestamp = DateTime.Now;
        // Arrange
        var command = new InsertOrderBookCommand("btcusd", timestamp, timestamp,null, null);

        //_orderBookRepositoryMock.Setup(x=>x.)

        var handler = new InsertOrderBookCommandHandler(_loggerInsertBookMock.Object, _orderBookRepositoryMock.Object, _mapper.Object);
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

        var handler = new InsertOrderBookCommandHandler(_loggerInsertBookMock.Object, _orderBookRepositoryMock.Object, _mapper.Object);
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

        var handler = new InsertOrderBookCommandHandler(_loggerInsertBookMock.Object, _orderBookRepositoryMock.Object, _mapper.Object);
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
            bids.Add(new BookLevelCommand() { Amount = i + 0.1544, Price = i + 0.454, Side = OrderBookSide.Bid });
            asks.Add(new BookLevelCommand() { Amount = i + 0.1544, Price = i + 0.454, Side = OrderBookSide.Ask });
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

        //_orderBookRepositoryMock.Setup(x=>x.CreateOrderBook()
        var handler = new InsertOrderBookCommandHandler(_loggerInsertBookMock.Object, _orderBookRepositoryMock.Object, _mapper.Object);

        // Action
        Result<bool> result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    
    #endregion

    //    #region Update
    //    [Fact]
    //    public async Task Handle_Should_ReturnFailureResult_When_Update_CashFlowIsNegative()
    //    {
    //        // Arrange
    //        var command = new UpdateCashFlowCommand("6596e8430a28df8491b77420", "Teste de fluxo de caixa", -500.00, CashFlowEntry.Debit, DateTime.Now);

    //        _cashFlowRepositoryMock.Setup(x => x.AddCashFlow(It.IsAny<CashFlow>()))
    //            .ReturnsAsync(false);

    //        var handler = new CashFlowCommandHandler(_cashFlowRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorHandlerMock.Object, _notifications, _loggerMock.Object);
    //        // Action
    //        Result<bool> result = await handler.Handle(command, default);

    //        // Assert
    //        result.IsFailed.Should().BeTrue();

    //    }

    //    [Fact]
    //    public async Task Handle_Should_ReturnFailureResult_When_Update_CashFlowIsZero()
    //    {
    //        // Arrange
    //        var command = new UpdateCashFlowCommand("6596e8430a28df8491b77420", "Teste de fluxo de caixa", 0, CashFlowEntry.Debit, DateTime.Now);

    //        _cashFlowRepositoryMock.Setup(x => x.AddCashFlow(It.IsAny<CashFlow>()))
    //            .ReturnsAsync(false);

    //        var handler = new CashFlowCommandHandler(_cashFlowRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorHandlerMock.Object, _notifications, _loggerMock.Object);
    //        // Action
    //        Result<bool> result = await handler.Handle(command, default);

    //        // Assert
    //        result.IsFailed.Should().BeTrue();
    //        //result.Error.Should().Be(
    //    }

    //    [Fact]
    //    public async Task Handle_Should_ReturnFailureResult_When_Update_CashFlowWithoutDescription()
    //    {
    //        // Arrange
    //        var command = new UpdateCashFlowCommand("6596e85952a677e5a9d1e039", string.Empty, 0, CashFlowEntry.Debit, DateTime.Now);

    //        _cashFlowRepositoryMock.Setup(x => x.AddCashFlow(It.IsAny<CashFlow>()))
    //            .ReturnsAsync(false);

    //        var handler = new CashFlowCommandHandler(_cashFlowRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorHandlerMock.Object, _notifications, _loggerMock.Object);
    //        // Action
    //        Result<bool> result = await handler.Handle(command, default);

    //        // Assert
    //        result.IsFailed.Should().BeTrue();
    //        //result.Error.Should().Be(
    //    }


    //    [Fact]
    //    public async Task Handle_Should_ReturnSuccessResult_When_Update_CashFlowIsPositivo()
    //    {
    //        // Arrange
    //        var command = new UpdateCashFlowCommand("6596e85952a677e5a9d1e039", "Teste de fluxo de caixa", 500.00, CashFlowEntry.Debit, DateTime.Now);

    //        _cashFlowRepositoryMock.Setup(x => x.AddCashFlow(It.IsAny<CashFlow>()))
    //            .ReturnsAsync(true);


    //        var handler = new CashFlowCommandHandler(_cashFlowRepositoryMock.Object,
    //            _unitOfWorkMock.Object,
    //            _mediatorHandlerMock.Object,
    //            _notifications,
    //            _loggerMock.Object);

    //        // Action
    //        Result<bool> result = await handler.Handle(command, default);

    //        // Assert
    //        result.IsSuccess.Should().BeTrue();
    //        //result.Errors.Should()..Be(true);
    //        //result.Error.Should().Be(
    //    }

    //    [Fact]
    //    public async Task Handle_Should_ReturnSuccessResult_When_Update_CashFlowGreaterThenZero()
    //    {
    //        // Arrange
    //        var command = new UpdateCashFlowCommand("6596e85952a677e5a9d1e039", "Teste de fluxo de caixa", 10, CashFlowEntry.Debit, DateTime.Now);

    //        _cashFlowRepositoryMock.Setup(x => x.AddCashFlow(It.IsAny<CashFlow>()))
    //            .ReturnsAsync(true);

    //        var handler = new CashFlowCommandHandler(_cashFlowRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorHandlerMock.Object, _notifications, _loggerMock.Object);
    //        // Action
    //        Result<bool> result = await handler.Handle(command, default);

    //        // Assert
    //        result.IsSuccess.Should().BeTrue();
    //        //result.Error.Should().Be(
    //    }

    //    [Fact]
    //    public async Task Handle_Should_ReturnSuccessResult_When_Update_CashFlowWithoutDescription()
    //    {
    //        // Arrange
    //        var command = new UpdateCashFlowCommand("6596e85952a677e5a9d1e039", "Teste de fluxo de caixa", 0, CashFlowEntry.Debit, DateTime.Now);

    //        _cashFlowRepositoryMock.Setup(x => x.AddCashFlow(It.IsAny<CashFlow>()))
    //            .ReturnsAsync(true);

    //        var handler = new CashFlowCommandHandler(_cashFlowRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorHandlerMock.Object, _notifications, _loggerMock.Object);
    //        // Action
    //        Result<bool> result = await handler.Handle(command, default);

    //        // Assert
    //        result.IsSuccess.Should().BeTrue();

    //    }
    //    #endregion
}
