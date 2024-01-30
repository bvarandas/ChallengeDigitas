using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderBook.Application.Commands;
using OrderBook.Core.AggregateObjects;
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
        var validation = command.Validation();
        if (!validation.IsValid)
        {
            return Result.Fail(validation.ToString("-"));
        }

        var orderBookToInsert = _mapper.Map<InsertOrderBookCommand, OrderBookRoot>(command);
        await _orderBookRepository.CreateOrderBookAsync(orderBookToInsert);
        _logger.LogInformation($"Order Book {orderBookToInsert?.Ticker}");
        return Result.Ok(true);
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
        var validation = command.Validation();

        if (!validation.IsValid)
        {
            return Result.Fail(validation.ToString("-"));
        }
        var orderBookToInsert = _mapper.Map<InsertOrderTradeCommand, OrderTrade>(command);
        //orderBookToInsert.Id = ObjectId.GenerateNewId().ToString();
        await _orderTradeRepository.CreateOrderTradeAsync(orderBookToInsert);
        _logger.LogInformation($"Order Trade {orderBookToInsert?.Ticker}");
        return Result.Ok(true);
    }
}