using AutoMapper;
using Microsoft.Extensions.Logging;
using OrderBook.Application.Commands;
using OrderBook.Application.Handlers;
using OrderBook.Application.Interfaces;
using OrderBook.Application.Responses.Books;
using OrderBook.Application.ViewModel;
using OrderBook.Core.Repositories;
using System.Collections.Concurrent;
namespace OrderBook.Application;
public class OrderBookService : IOrderBookService
{
    private readonly ILogger<OrderBookService> _logger;
    private readonly IMapper _mapper;
    private readonly IOrderBookRepository _orderBookRepository;
    private readonly IOrderTradeRepository _orderTradeRepository;
    private ConcurrentDictionary<string, Application.Responses.Books.OrderBook> _dicOrderBook;
    private ConcurrentQueue<Application.Responses.Books.OrderBook> _queueOrderBook;
    private static ConcurrentDictionary<string, OrderBookDataViewModel> _dicOrderBookData;
    private readonly Thread _threadOrderBook, _treadDequequeOrderBook;
    private readonly SemaphoreSlim _semaphore;
    public OrderBookService(IMapper mapper, 
        IOrderBookRepository orderBookRepository,
        IOrderTradeRepository orderTradeRepository,
        ILogger<OrderBookService> logger)
    {
        _mapper = mapper;
        _orderBookRepository = orderBookRepository;
        _orderTradeRepository = orderTradeRepository;
        _logger = logger;
        
        _dicOrderBook = new ConcurrentDictionary<string, Responses.Books.OrderBook>();
        _dicOrderBookData = new ConcurrentDictionary<string, OrderBookDataViewModel>();
        _semaphore = new SemaphoreSlim(1, 2);

        _threadOrderBook = new Thread(new ThreadStart(OrderBookCacheAsync));
        _threadOrderBook.Name = "OrderBookCacheAsync";
        _threadOrderBook.Start();

        _treadDequequeOrderBook = new Thread(new ThreadStart(DequeueOrderBookCacheAsync));
        _treadDequequeOrderBook.Name = "DequeueOrderBookCacheAsync";
        _treadDequequeOrderBook.Start();
    }

    private async Task<(IList<BookLevel>, double)> GetQuotesBidAsync(Core.ValuesObject.Ticker ticker,double quantityRequest)
    {
        var result = (new List<BookLevel>(), 0.0);
        double quantityCollected = 0.0;
        if (_dicOrderBook.TryGetValue(ticker.ticker, out Responses.Books.OrderBook orderBook))
        {
            Array.ForEach(orderBook.Bids, bid => { 
                if (quantityRequest < quantityCollected)
                {
                    quantityCollected += bid.Amount;
                    result.Item1.Add(bid);
                }
            });
            result.Item2 = quantityCollected;
        }

        return result;
    }

    private async Task<(IList<BookLevel>, double)> GetQuotesAskAsync(Core.ValuesObject.Ticker ticker,double quantityRequest)
    {
        var result = (new List<BookLevel>(), 0.0);
        double AmountCollected = 0.0;
        if (_dicOrderBook.TryGetValue(ticker.ticker, out Responses.Books.OrderBook orderBook))
        {
            Array.ForEach(orderBook.Asks, ask => {
                if (quantityRequest < AmountCollected)
                {
                    AmountCollected += ask.Amount;
                    result.Item1.Add(ask);
                }
            });
        }
        return result;
    }

    private async void DequeueOrderBookCacheAsync()
    {
        while (true)
        {
            await _semaphore.WaitAsync();
           
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

            _semaphore.Release();
            
            Thread.Sleep(1000);
        }
    }

    private async void OrderBookCacheAsync()
    {
        TimeSpan lastCycle = DateTime.Now.TimeOfDay;
        while (true)
        {
            DateTime now = DateTime.Now;
            
            await _semaphore.WaitAsync();
            
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
            kvp.Value.Ticker    = orderBook.Ticker;
            kvp.Value.MinPrice  = orderBook.Bids.FirstOrDefault().Price;
            kvp.Value.MaxPrice  = orderBook.Asks.FirstOrDefault().Price;
            
            var average         = (orderBook.Asks.Take(100).Average(x => x.Price) + orderBook.Bids.Take(100).Average(x => x.Price))/2;
            var average5Seconds = (orderBook.Asks.Average(x => x.Price) + orderBook.Bids.Average(x => x.Price)) / 2;
            var averageQuanity  = (orderBook.Asks.Average(x => x.Amount) + orderBook.Bids.Average(x => x.Amount)) / 2;

            kvp.Value.AveragePrice              = average;
            kvp.Value.AveragePriceLast5Seconds  = average5Seconds;
            kvp.Value.AverageAmountQuantity     = averageQuanity;
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
                GetQuotesAskAsync(command.Ticker,command.QuantityRequested) : 
                GetQuotesBidAsync(command.Ticker, command.QuantityRequested);

            var entity = _mapper.Map<OrderTradeCommand, Core.Entities.OrderTrade>(command);
            var listBookLevel = _mapper.Map<IList<OrderBook.Application.Responses.Books.BookLevel>, IList<OrderBook.Core.Entities.BookLevel>>(quotations.Result.Item1);
            entity.Quotes = listBookLevel;
            entity.AmountShaved = quotations.Result.Item2;
            var result = await _orderTradeRepository.CreateOrderTradeAsync(entity);
            resultObject = _mapper.Map<OrderBook.Core.Entities.OrderTrade, OrderTradeViewModel>(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return resultObject;
    }
}

class AsksComparer : IComparer<BookLevel>
{
    public int Compare(BookLevel? x, BookLevel? y)
    {
        if (x.Price == y.Price) return 0;
        if (x.Price > y.Price) return -1;
        return 1;
    }
}

class BidsComparer : IComparer<BookLevel>
{
    public int Compare(BookLevel? x, BookLevel? y)
    {
        if (x.Price == y.Price) return 0;
        if (x.Price < y.Price) return -1;
        return 1;
    }
}
