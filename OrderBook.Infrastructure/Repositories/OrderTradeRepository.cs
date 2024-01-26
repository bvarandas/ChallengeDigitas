using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OrderBook.Core.Entities;
using OrderBook.Core.Repositories;
using OrderBook.Infrastructure.Data;

namespace OrderBook.Infrastructure.Repositories;

public class OrderTradeRepository : IOrderTradeRepository
{
    private readonly IOrderTradeContext _context;
    private readonly ILogger<OrderTradeRepository> _logger;
    public OrderTradeRepository(IOrderTradeContext context, ILogger<OrderTradeRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<bool> CreateOrderTradeAsync(OrderTrade orderTrade)
    {
        string ticker = orderTrade.Ticker.ticker;
        var inserts = new List<WriteModel<Core.Entities.OrderTrade>>();
        var filterBuilder = Builders<Core.Entities.OrderTrade>.Filter;
        bool result = false;
        var filter = filterBuilder.Where(x => x.Ticker.ticker == ticker);
        try
        {
            inserts.Add(new InsertOneModel<Core.Entities.OrderTrade>(orderTrade));

            var insertResult = await _context.OrderTrade.BulkWriteAsync(inserts);
            result = insertResult.IsAcknowledged && insertResult.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }
}