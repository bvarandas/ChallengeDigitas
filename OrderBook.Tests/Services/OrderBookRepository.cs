using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using OrderBook.Core.Repositories;
using AutoMapper;
using FluentAssertions;
using OrderBook.Infrastructure.Repositories;

namespace OrderBook.Tests.Services;

public class OrderBookRepository
{
    private readonly Mock<IOrderBookRepository> _orderBookRepositoryMock;
    private readonly Mock<IOrderTradeRepository> _orderTradeRepositoryMock;
    private readonly Mock<ILogger<OrderTradeRepository>> _loggerOrderTradeMock;
    private readonly Mock<ILogger<OrderBookRepository>> _loggerOrderBookMock;
    private readonly Mock<IMapper> _mapper;

    public OrderBookRepository()
    {
        _orderTradeRepositoryMock = new();
        _orderBookRepositoryMock = new();
        _loggerOrderTradeMock = new();
        _loggerOrderBookMock = new();
    }

}
