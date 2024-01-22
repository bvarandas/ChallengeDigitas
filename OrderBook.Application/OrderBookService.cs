using AutoMapper;
using Microsoft.Extensions.Logging;
using OrderBook.Application.Commands;
using OrderBook.Application.Handlers;
using OrderBook.Application.Interfaces;
using OrderBook.Application.ViewModel;
using OrderBook.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Application;

public class OrderBookService : IOrderBookService
{
    private readonly ILogger<OrderBookService> _logger;
    private readonly IMapper _mapper;
    private readonly IOrderBookRepository _orderBookRepository;
    public OrderBookService(IMapper mapper, IOrderBookRepository orderBookRepository, ILogger<OrderBookService> logger)
    {
        _mapper = mapper;
        _orderBookRepository = orderBookRepository;
        _logger = logger;
    }
    public Task<OrderBookModel> GetCashOrderBookIDAsync(string orderBookId)
    {
        throw new NotImplementedException();
    }

    public Task<IAsyncEnumerable<OrderBookModel>> GetListAllAsync()
    {
        throw new NotImplementedException();
    }

}
