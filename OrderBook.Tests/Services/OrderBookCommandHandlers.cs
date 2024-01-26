//using FluentResults;
//using Microsoft.Extensions.Logging;
//using Moq;
//using OrderBook.Application.Handlers;
//using OrderBook.Core.Repositories;
//using OrderBook.Application.Commands;

//namespace OrderBook.Tests.Services;

//public class OrderBookCommandHandlers
//{
//    private readonly Mock<IOrderBookRepository> _cashFlowRepositoryMock;
//    private readonly Mock<ILogger<InsertOrderTradeCommandHandler>> _loggerUpdateTradeMock;
//    private readonly Mock<ILogger<InsertOrderBookCommandHandler>> _loggerInsertBookMock;
//    public OrderBookCommandHandlers()
//    {
//        _cashFlowRepositoryMock = new();
//        _loggerUpdateTradeMock = new();
//        _loggerInsertBookMock = new();
//    }

//    #region Insert

//    [Fact]
//    public async Task Handle_Should_ReturnFailureResult_When_Insert_CashFlowIsNegative()
//    {
//        var timestamp = DateTime.Now.TimeOfDay;
//        // Arrange
//        var command = new InsertOrderBookCommand("btcusd", timestamp, timestamp, DateTime.Now.TimeOfDay, -500.00, CashFlowEntry.Debit, DateTime.Now);

//        //_cashFlowRepositoryMock.Setup(x=>x.)

//        var handler = new OrderBookCommandHandler(_cashFlowRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorHandlerMock.Object, _notifications, _loggerMock.Object);
//        // Action
//        Result<bool> result = await handler.Handle(command, default);

//        // Assert
//        result.IsFailed.Should().BeTrue();

//    }

//    [Fact]
//    public async Task Handle_Should_ReturnFailureResult_When_Insert_CashFlowIsZero()
//    {
//        // Arrange
//        var command = new InsertCashFlowCommand("Teste de fluxo de caixa", 0, CashFlowEntry.Debit, DateTime.Now);

//        //_cashFlowRepositoryMock.Setup(x=>x.)

//        var handler = new CashFlowCommandHandler(_cashFlowRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorHandlerMock.Object, _notifications, _loggerMock.Object);
//        // Action
//        Result<bool> result = await handler.Handle(command, default);

//        // Assert
//        result.IsFailed.Should().BeTrue();
//        //result.Error.Should().Be(
//    }

//    [Fact]
//    public async Task Handle_Should_ReturnFailureResult_When_Insert_CashFlowWithoutDescription()
//    {
//        // Arrange
//        var command = new InsertCashFlowCommand(string.Empty, 0, CashFlowEntry.Debit, DateTime.Now);

//        //_cashFlowRepositoryMock.Setup(x=>x.)

//        var handler = new CashFlowCommandHandler(_cashFlowRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorHandlerMock.Object, _notifications, _loggerMock.Object);
//        // Action
//        Result<bool> result = await handler.Handle(command, default);

//        // Assert
//        result.IsFailed.Should().BeTrue();
//        //result.Error.Should().Be(
//    }


//    [Fact]
//    public async Task Handle_Should_ReturnSuccessResult_When_Insert_CashFlowIsPositive()
//    {
//        // Arrange
//        var command = new InsertCashFlowCommand("Teste de fluxo de caixa", 500.00, CashFlowEntry.Debit, DateTime.Now);

//        //_cashFlowRepositoryMock.Setup(x=>x.)

//        // command.IsValid();

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
//    public async Task Handle_Should_ReturnSuccessResult_When_Insert_CashFlowGreaterThenZero()
//    {
//        // Arrange
//        var command = new InsertCashFlowCommand("Teste de fluxo de caixa", 10, CashFlowEntry.Debit, DateTime.Now);

//        //_cashFlowRepositoryMock.Setup(x=>x.)

//        var handler = new CashFlowCommandHandler(_cashFlowRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorHandlerMock.Object, _notifications, _loggerMock.Object);
//        // Action
//        Result<bool> result = await handler.Handle(command, default);

//        // Assert
//        result.IsSuccess.Should().BeTrue();
//        //result.Error.Should().Be(
//    }

//    [Fact]
//    public async Task Handle_Should_ReturnSuccessResult_When_Insert_CashFlowWithoutDescription()
//    {
//        // Arrange
//        var command = new InsertCashFlowCommand("Teste de fluxo de caixa", 0, CashFlowEntry.Debit, DateTime.Now);

//        //_cashFlowRepositoryMock.Setup(x=>x.)

//        var handler = new CashFlowCommandHandler(_cashFlowRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorHandlerMock.Object, _notifications, _loggerMock.Object);
//        // Action
//        Result<bool> result = await handler.Handle(command, default);

//        // Assert
//        result.IsFailed.Should().BeTrue();
//        //result.Error.Should().Be(
//    }
//    #endregion

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
//}
