using FluentResults;
using OrderBook.Application.Commands;
using OrderBook.Application.ViewModel;
namespace OrderBook.Application.Interfaces;
public interface IOrderBookService
{
    Task<Result<bool>> AddOrderBookCacheAsync(Application.Responses.Books.OrderBook orderBook);
    Task<OrderBookDataViewModel> GetOrderBookDataCacheAsync(string ticker);
    Task<OrderTradeViewModel> SendOrderTradeAsync(OrderTradeCommand command);
}