using OrderBook.Application.Commands;
using OrderBook.Application.ViewModel;
namespace OrderBook.Application.Interfaces;
public interface IOrderBookService
{
    Task<IAsyncEnumerable<OrderBookViewModel>> GetListAllAsync();
    Task<OrderBookViewModel> GetCashOrderBookIDAsync(string orderBookId);
    Task AddOrderBookCacheAsync(Application.Responses.Books.OrderBook orderBook);
    Task<OrderBookDataViewModel> GetOrderBookDataCacheAsync(string ticker);
    Task<OrderTradeViewModel> OrderTradeAsync(OrderTradeCommand command);
}