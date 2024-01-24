using AutoMapper;
using OrderBook.Application.ViewModel;
namespace OrderBook.Application.Automapper;
public class DomainToViewModelMappingProfile : Profile
{
    public DomainToViewModelMappingProfile()
    {
        CreateMap<OrderBook.Core.Entities.OrderBook, OrderBookViewModel>();
        CreateMap<OrderBook.Core.Entities.BookLevel, BookLevelViewModel>();
            //.ConstructUsing(c => new OrderBookViewModel(c.Ticker, c.Timestamp, c.Microtimestamp, bids, asks));
    }
}