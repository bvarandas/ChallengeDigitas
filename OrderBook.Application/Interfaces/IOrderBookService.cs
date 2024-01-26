﻿using OrderBook.Application.Commands;
using OrderBook.Application.ViewModel;
namespace OrderBook.Application.Interfaces;
public interface IOrderBookService
{
    Task<IAsyncEnumerable<OrderBookViewModel>> GetListAllAsync();
    Task<OrderBookViewModel> GetCashOrderBookIDAsync(string orderBookId);
    Task<OrderTradeViewModel> OrderTradeAsync(OrderTradeCommand command);
    Task AddOrderBookCacheAsync(Application.Responses.Books.OrderBook orderBook);
}