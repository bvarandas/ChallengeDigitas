using AutoMapper;
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

    private async Task<(IList<BookLevel>, double)> GetQuotesBidAsync(string ticker,double quantityRequest)
    {
        var result = (new List<BookLevel>(), 0.0);
        double quantityCollected = 0.0;
        if (_dicOrderBook.TryGetValue(ticker, out Responses.Books.OrderBook orderBook))
        {
            Array.ForEach(orderBook.Bids, bid => { 
                if (quantityRequest > quantityCollected && (quantityCollected + bid.Amount) < quantityRequest)
                {
                    quantityCollected += bid.Amount;
                    result.Item1.Add(bid);
                }
            });
            result.Item2 = quantityCollected;
        }

        return result;
    }

    private async Task<(IList<BookLevel>, double)> GetQuotesAskAsync(string ticker,double quantityRequest)
    {
        var result = (new List<BookLevel>(), 0.0);
        double AmountCollected = 0.0;
        if (_dicOrderBook.TryGetValue(ticker, out Responses.Books.OrderBook orderBook))
        {
            Array.ForEach(orderBook.Asks, ask => {
                if (quantityRequest > AmountCollected && (AmountCollected + ask.Amount) < quantityRequest)
                {
                    AmountCollected += ask.Amount;
                    result.Item1.Add(ask);
                }
            });
        }
        result.Item2 = AmountCollected;
        return result;
    }

    private async void DequeueOrderBookCacheAsync()
    {
        while (true)
        {
            while (_queueOrderBook.TryDequeue(out var orderBook))
            {
                var now = DateTime.Now;

                var book= _dicOrderBook[orderBook.Ticker];

                var listAsk = book.Asks.ToList();
                var listBids = book.Bids.ToList();
                orderBook.Asks.ToList().ForEach(x =>
                {
                    x.Timestamp = now;
                    listAsk.Add(x);
                });
                orderBook.Bids.ToList().ForEach(x =>
                {
                    x.Timestamp = now;
                    listBids.Add(x);
                });
                book.Asks = listAsk.ToArray();
                book.Bids = listBids.ToArray();
            }
            //_semaphore.Wait();
            //_semaphore.Release();
            
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
            if (orderBook.Bids.Length == 0 || orderBook.Asks.Length == 0) continue;
            kvp.Value.Ticker    = orderBook.Ticker;
            kvp.Value.MinPrice  = orderBook.Bids.FirstOrDefault().Price;
            kvp.Value.MaxPrice  = orderBook.Asks.FirstOrDefault().Price;
            
            var average         = (orderBook.Asks.Take(100).Average(x => x.Price) + orderBook.Bids.Take(100).Average(x => x.Price))/2;
            var average5Seconds = (orderBook.Asks.Average(x => x.Price) + orderBook.Bids.Average(x => x.Price)) / 2;
            var averageQuantity  = (orderBook.Asks.Average(x => x.Amount) + orderBook.Bids.Average(x => x.Amount)) / 2;

            kvp.Value.AveragePrice              = average;
            kvp.Value.AveragePriceLast5Seconds  = average5Seconds;
            kvp.Value.AverageAmountQuantity     = averageQuantity;
        }
    }
    private async Task SortOrderBookCacheAsync()
    {
        foreach (KeyValuePair<string, Responses.Books.OrderBook> kvp in _dicOrderBook)
        {
            Array.Sort(kvp.Value.Asks, new AsksComparer());
            Array.Sort(kvp.Value.Bids, new BidsComparer());
        }
    }
    private async Task RemoveOldOrderBookCacheAsync()
    {
        var now = DateTime.Now;
        foreach (KeyValuePair<string, Responses.Books.OrderBook> kvp in _dicOrderBook)
        {
            var asksToRemove = Array.FindAll( kvp.Value.Asks, x => x.Timestamp < now.AddSeconds(-5));
            var bidsToRemove = Array.FindAll(kvp.Value.Bids, x => x.Timestamp < now.AddSeconds(-5));

            kvp.Value.Asks = kvp.Value.Asks.Except(asksToRemove).ToArray();
            kvp.Value.Bids = kvp.Value.Bids.Except(bidsToRemove).ToArray();
        }
    }
    public async Task<OrderBookDataViewModel> GetOrderBookDataCacheAsync(string ticker)
    {
        return _dicOrderBookData[ticker]; 
    }
    
    public async Task AddOrderBookCacheAsync(Application.Responses.Books.OrderBook orderBook)
    {
        if (!_dicOrderBook.TryGetValue(orderBook.Ticker, out Responses.Books.OrderBook book))
        {
            _dicOrderBook.TryAdd(orderBook.Ticker, orderBook);
            _dicOrderBookData.TryAdd(orderBook.Ticker, new OrderBookDataViewModel());
        }
        _queueOrderBook.Enqueue(orderBook);
    }

    public Task<OrderBookViewModel> GetCashOrderBookIDAsync(string orderBookId)
    {
        throw new NotImplementedException();
    }

    public Task<IAsyncEnumerable<OrderBookViewModel>> GetListAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<OrderTradeViewModel> OrderTradeAsync(OrderTradeCommand command)
    {
        OrderTradeViewModel resultObject = null!;
        try
        {
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
