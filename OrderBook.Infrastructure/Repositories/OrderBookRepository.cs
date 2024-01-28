using OrderBook.Core.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using OrderBook.Core.Specs;
using OrderBook.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using OrderBook.Core.AggregateObjects;

namespace OrderBook.Infrastructure.Repositories;

public class OrderBookRepository : IOrderBookRepository
{
    private readonly IOrderBookContext _context;
    private readonly ILogger<OrderBookRepository> _logger;
    public OrderBookRepository(IOrderBookContext context, ILogger<OrderBookRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> CreateOrderBook(Core.AggregateObjects.OrderBookRoot orderBook)
    {
        string ticker = orderBook.Ticker;
        var inserts = new List<WriteModel<OrderBookRoot>>();
        var filterBuilder = Builders<OrderBookRoot>.Filter;
        bool result = false;
        var filter = filterBuilder.Where(x => x.Ticker == ticker);
        try
        {
            inserts.Add(new InsertOneModel<OrderBookRoot>(orderBook));

            var insertResult = await _context.OrderBooks.BulkWriteAsync(inserts);
            result = insertResult.IsAcknowledged && insertResult.ModifiedCount > 0;
            
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    public async Task<IEnumerable<OrderBookRoot>> GetOrderBooks(OrderBookSpecParams specParams)
    {
        var builder = Builders<OrderBookRoot>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrEmpty(specParams.Search))
        {
            var searchFilter = builder.Regex(x => x.Ticker, new BsonRegularExpression(specParams.Search));
            filter &= searchFilter;
        }
        return _context.OrderBooks.Find(filter).ToEnumerable();
    }

    public async Task<bool> UpdateOrderBook(OrderBookRoot orderBook)
    {
        string ticker = orderBook.Ticker;
        var updates = new List<WriteModel<OrderBookRoot>>();
        var filterBuilder = Builders<OrderBookRoot>.Filter;

        var filter = filterBuilder.Where(x => x.Ticker == ticker);
        updates.Add(new ReplaceOneModel<OrderBookRoot>(filter, orderBook));

        var updateResult = await _context.OrderBooks.BulkWriteAsync(updates);

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }
}
