using Amazon.Runtime.Internal;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OrderBook.Application.Commands;
using OrderBook.Core.Repositories;
using OrderBook.Infrastructure.Repositories;

namespace OrderBook.Application.Handlers;

public class UpdateOrderBookCommandHandler : IRequestHandler<UpdateOrderBookCommand, Result<bool>>
{
    private readonly ILogger<UpdateOrderBookCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IOrderBookRepository _orderBookRepository;

    public UpdateOrderBookCommandHandler(ILogger<UpdateOrderBookCommandHandler> logger, IOrderBookRepository orderBookRepository, IMapper mapper)
    {
        _logger = logger;   
        _orderBookRepository = orderBookRepository;
        _mapper = mapper;
    }

    public async Task<Result<bool>> Handle(UpdateOrderBookCommand command, CancellationToken cancellationToken)
    {

        var orderBookToUpdate = new Core.Entities.OrderBook();
        _mapper.Map(command, orderBookToUpdate, typeof(UpdateOrderBookCommand), typeof(Core.Entities.OrderBook));
        var resultUpdate = await _orderBookRepository.UpdateOrderBook(orderBookToUpdate);
        _logger.LogInformation($"Order Book {orderBookToUpdate.Ticker}");

        return await Task.FromResult(resultUpdate);
    }
}


public class InsertOrderBookCommandHandler : IRequestHandler<InsertOrderBookCommand, Result<bool>>
{
    private readonly ILogger<UpdateOrderBookCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IOrderBookRepository _orderBookRepository;

    public InsertOrderBookCommandHandler(ILogger<UpdateOrderBookCommandHandler> logger, IOrderBookRepository orderBookRepository, IMapper mapper)
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