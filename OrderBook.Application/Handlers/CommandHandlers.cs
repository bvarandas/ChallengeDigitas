using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderBook.Application.Commands;
using OrderBook.Core.Repositories;
namespace OrderBook.Application.Handlers;
public class InsertOrderBookCommandHandler : IRequestHandler<InsertOrderBookCommand, Result<bool>>
{
    private readonly ILogger<InsertOrderBookCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IOrderBookRepository _orderBookRepository;

    public InsertOrderBookCommandHandler(ILogger<InsertOrderBookCommandHandler> logger, IOrderBookRepository orderBookRepository, IMapper mapper)
    {
        _mapper = mapper;
        _logger = logger;
        _orderBookRepository = orderBookRepository;
    }

    public async Task<Result<bool>> Handle(InsertOrderBookCommand command, CancellationToken cancellationToken)
    {
        var orderBookToInsert = _mapper.Map<InsertOrderBookCommand, OrderBook.Core.Entities.OrderBook>(command);
        await _orderBookRepository.CreateOrderBook(orderBookToInsert);
        _logger.LogInformation($"Order Book {orderBookToInsert.Ticker}");
        return await Task.FromResult(true);
    }
}

public class InsertOrderTradeCommandHandler : IRequestHandler<InsertOrderTradeCommand, Result<bool>>
{
    private readonly ILogger<InsertOrderTradeCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IOrderTradeRepository _orderTradeRepository;
    public InsertOrderTradeCommandHandler(ILogger<InsertOrderTradeCommandHandler> logger, IMapper mapper, IOrderTradeRepository orderTradeRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _orderTradeRepository = orderTradeRepository;
    }
    public async Task<Result<bool>> Handle(InsertOrderTradeCommand command, CancellationToken cancellationToken)
    {
        var orderBookToInsert = _mapper.Map<InsertOrderTradeCommand, OrderBook.Core.Entities.OrderTrade>(command);

        await _orderTradeRepository.CreateOrderTradeAsync(orderBookToInsert);

        _logger.LogInformation($"Order Trade {orderBookToInsert.Ticker}");
        return await Task.FromResult(true);
    }
}