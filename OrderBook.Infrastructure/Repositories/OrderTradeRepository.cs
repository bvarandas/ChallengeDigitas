using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OrderBook.Core.AggregateObjects;
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
        bool result = false;
        try
        {
            string ticker = orderTrade.Ticker;
            var inserts = new List<WriteModel<OrderTrade>>();

            inserts.Add(new InsertOneModel<OrderTrade>(orderTrade));

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