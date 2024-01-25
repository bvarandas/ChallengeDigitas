using AutoMapper;
using Microsoft.Extensions.Logging;
using OrderBook.Application.Commands;
using OrderBook.Application.Handlers;
using OrderBook.Application.Interfaces;
using OrderBook.Application.ViewModel;
using OrderBook.Core.Repositories;
using System;
using System.Collections.Concurrent;
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
    private ConcurrentDictionary<Guid, Application.Responses.Books.OrderBook> _dicOrderBook;
    private readonly Thread _threadOrderBook;
    public OrderBookService(IMapper mapper, IOrderBookRepository orderBookRepository, ILogger<OrderBookService> logger)
    {
        _mapper = mapper;
        _orderBookRepository = orderBookRepository;
        _logger = logger;
        _dicOrderBook = new ConcurrentDictionary<Guid, Responses.Books.OrderBook>();
        //_threadOrderBook = new Thread(new ThreadStart(UpdateOrderBookCache));
    }
    //private async Task UpdateOrderBookCache()
    //{

    //}

    public Task<OrderBookViewModel> GetCashOrderBookIDAsync(string orderBookId)
    {
        throw new NotImplementedException();
    }

    public Task<IAsyncEnumerable<OrderBookViewModel>> GetListAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task OrderTrade(OrderTradeCommand command)
    {
        throw new NotImplementedException();
    }
}
