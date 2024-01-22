using OrderBook.Application.Commands;
using OrderBook.Application.ViewModel;
namespace OrderBook.Application.Interfaces;
public interface IOrderBookService
{
    Task<IAsyncEnumerable<OrderBookModel>> GetListAllAsync();
    Task<OrderBookModel> GetCashOrderBookIDAsync(string orderBookId);
}