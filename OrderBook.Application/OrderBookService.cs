using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OrderBook.Application.Commands;
using OrderBook.Application.Handlers;
using OrderBook.Application.Interfaces;
using OrderBook.Application.Responses.Books;
using OrderBook.Application.ViewModel;
using OrderBook.Core.AggregateObjects;
using OrderBook.Core.Repositories;
using OrderBook.Infrastructure.Repositories;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace OrderBook.Application;
public class OrderBookService : IOrderBookService
{
    private readonly ILogger<OrderBookService> _logger;
    private readonly IMapper _mapper;
    private ConcurrentDictionary<string, Application.Responses.Books.OrderBook> _dicOrderBook;
    private ConcurrentQueue<Application.Responses.Books.OrderBook> _queueOrderBook;
    private static ConcurrentDictionary<string, OrderBookDataViewModel> _dicOrderBookData;
    private readonly Thread _threadOrderBook, _treadDequequeOrderBook;
    private readonly SemaphoreSlim _semaphore;
    private readonly IMediator _mediator;
    public OrderBookService(IMapper mapper, 
        IMediator mediator,
        ILogger<OrderBookService> logger)
    {
        _mapper = mapper;
        _logger = logger;
        
        _mediator = mediator;

        _dicOrderBook = new ConcurrentDictionary<string, Responses.Books.OrderBook>();
        _dicOrderBookData = new ConcurrentDictionary<string, OrderBookDataViewModel>();
        _semaphore = new SemaphoreSlim(1, 2);

        _queueOrderBook = new ConcurrentQueue<Responses.Books.OrderBook>();

        _threadOrderBook = new Thread(new ThreadStart(OrderBookCacheAsync));
        _threadOrderBook.Name = "OrderBookCacheAsync";
        _threadOrderBook.Start();

        _treadDequequeOrderBook = new Thread(new ThreadStart(DequeueOrderBookCacheAsync));
        _treadDequequeOrderBook.Name = "DequeueOrderBookCacheAsync";
        _treadDequequeOrderBook.Start();

    }

    private async Task<(List<BookLevel>, double)> GetQuotesBidAsync(string ticker,double quantityRequest)
    {
        var result = (new List<BookLevel>(), 0.0);

        if (_dicOrderBook.TryGetValue(ticker, out Responses.Books.OrderBook orderBook))
        {
            result = orderBook.GetQuotesBidAsync(quantityRequest);
        }

        return result;
    }

    private async Task<(List<BookLevel>, double)> GetQuotesAskAsync(string ticker,double quantityRequest)
    {
        var result = (new List<BookLevel>(), 0.0);
        
        if (_dicOrderBook.TryGetValue(ticker, out Responses.Books.OrderBook orderBook))
        {
            result = orderBook.GetQuotesAskAsync(quantityRequest);
        }
        
        return result;
    }

    private async void DequeueOrderBookCacheAsync()
    {
        while (true)
        {
            while (_queueOrderBook.TryDequeue(out var orderBook))
            {
                var now = DateTime.Now;

                if (_dicOrderBook.TryGetValue(orderBook.Ticker, out Responses.Books.OrderBook book))
                {
                    await book.AddOrderBookCacheAsync(orderBook);
                }
            }
            
            Thread.Sleep(100);
        }
    }

    private async void OrderBookCacheAsync()
    {
        TimeSpan lastCycle = DateTime.Now.TimeOfDay;
        while (true)
        {
            DateTime now = DateTime.Now;
            
            _semaphore.Wait();
            
            if ((now.TimeOfDay - lastCycle).TotalSeconds >= 5)
            {
                lastCycle = now.TimeOfDay;
                await RemoveOldOrderBookCacheAsync();
                await SortOrderBookCacheAsync();
                await UpdateOrderBookDataCacheAsync();
            }
            _semaphore.Release();

            Thread.Sleep(1000);
        }
    }

    private async Task UpdateOrderBookDataCacheAsync()
    {
        foreach (KeyValuePair<string, OrderBookDataViewModel> kvp in _dicOrderBookData)
        {
            var orderBook       = _dicOrderBook[kvp.Key];
            
            if (orderBook.Bids.Length == 0 || orderBook.Asks.Length == 0) 
                continue;
            
            await kvp.Value.UpdateOrderBookDataCacheAsync(orderBook);
        }
    }
    private async Task SortOrderBookCacheAsync()
    {
        foreach (KeyValuePair<string, Responses.Books.OrderBook> kvp in _dicOrderBook)
        {
            await kvp.Value.SortOrderBookCacheAsync();
        }
    }
    private async Task RemoveOldOrderBookCacheAsync()
    {
        var now = DateTime.Now;
        foreach (KeyValuePair<string, Responses.Books.OrderBook> kvp in _dicOrderBook)
        {
            await kvp.Value.RemoveOldOrderBookCacheAsync();
        }
    }
    public async Task<OrderBookDataViewModel> GetOrderBookDataCacheAsync(string ticker)
    {
        return _dicOrderBookData[ticker]; 
    }
    
    public async Task<Result<bool>> AddOrderBookCacheAsync(Application.Responses.Books.OrderBook orderBook)
    {
        if (orderBook is null && !string.IsNullOrEmpty(orderBook.Ticker))
            return Result.Fail("O objeto nem o Ticker não pode ser nulo");

        if (orderBook?.Bids is null || orderBook?.Asks is null)
            return Result.Fail("O Bids or aks não podem ser nulos");

        if (!_dicOrderBook.TryGetValue(orderBook.Ticker, out Responses.Books.OrderBook book))
        {
            _dicOrderBook.TryAdd(orderBook.Ticker, orderBook);
            _dicOrderBookData.TryAdd(orderBook.Ticker, new OrderBookDataViewModel());
        }
        _queueOrderBook.Enqueue(orderBook);
        return Result.Ok(true);
    }

    public async Task<OrderTradeViewModel> SendOrderTradeAsync(OrderTradeCommand command)
    {
        var resultObject = new OrderTradeViewModel();
        try
        {
            if (command.QuantityRequested <= 0)
                throw new Exception("Quantidade não pode ser 0 ou menor que zero");

            var quotations = command.TradeSide == Core.Enumerations.TradeSide.Buy ?
                GetQuotesAskAsync(command.Ticker, command.QuantityRequested) :
                GetQuotesBidAsync(command.Ticker, command.QuantityRequested);

            
            var entity = _mapper.Map<OrderTradeCommand, OrderTrade>(command);
            var listBookLevel = _mapper.Map<IList<OrderBook.Application.Responses.Books.BookLevel>, IList<BookLevelCommand>>(quotations.Result.Item1);
            double totalPriceShaved = 0.0;

            listBookLevel.ToList().ForEach(quote=> { totalPriceShaved += (quote.Amount * quote.Price);});
            
            var amountShaved = quotations.Result.Item2;
            var insertCommand = new InsertOrderTradeCommand(ObjectId.GenerateNewId().ToString(), command.Ticker, command.QuantityRequested, command.TradeSide, listBookLevel, amountShaved, totalPriceShaved);
            var result = _mediator.Send(insertCommand);
            
            resultObject = _mapper.Map<InsertOrderTradeCommand, OrderTradeViewModel>(insertCommand);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return resultObject;
    }
}

public class AsksComparer : IComparer<BookLevel>
{
    public int Compare(BookLevel? x, BookLevel? y)
    {
        if (x.Price == y.Price) return 0;
        if (x.Price > y.Price) return -1;
        return 1;
    }
}

public class BidsComparer : IComparer<BookLevel>
{
    public int Compare(BookLevel? x, BookLevel? y)
    {
        if (x.Price == y.Price) return 0;
        if (x.Price < y.Price) return -1;
        return 1;
    }
}
