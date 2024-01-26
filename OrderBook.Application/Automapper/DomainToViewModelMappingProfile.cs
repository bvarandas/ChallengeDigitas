using AutoMapper;
using OrderBook.Application.Responses.Books;
using OrderBook.Application.ViewModel;
namespace OrderBook.Application.Automapper;
public class DomainToViewModelMappingProfile : Profile
{
    public DomainToViewModelMappingProfile()
    {
        CreateMap<OrderBook.Core.Entities.OrderBook, OrderBookViewModel>();
        CreateMap<OrderBook.Core.Entities.BookLevel, BookLevelViewModel>();
        CreateMap<OrderBook.Core.Entities.BookLevel, BookLevel>();
        CreateMap<OrderBook.Core.Entities.OrderTrade, OrderTradeViewModel>();
    }
}