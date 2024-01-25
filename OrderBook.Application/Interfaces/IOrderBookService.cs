using OrderBook.Application.Commands;
using OrderBook.Application.ViewModel;
namespace OrderBook.Application.Interfaces;
public interface IOrderBookService
{
    Task<IAsyncEnumerable<OrderBookViewModel>> GetListAllAsync();
    Task<OrderBookViewModel> GetCashOrderBookIDAsync(string orderBookId);
    Task OrderTrade(OrderTradeCommand command);
}