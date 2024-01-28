using AutoMapper;
using OrderBook.Application.Commands;
using OrderBook.Application.Responses.Books;
using OrderBook.Application.ViewModel;
using OrderBook.Core.AggregateObjects;

namespace OrderBook.Application.Automapper;
public class DomainToViewModelMappingProfile : Profile
{
    public DomainToViewModelMappingProfile()
    {
        CreateMap<OrderBookRoot, OrderBookViewModel>();
        CreateMap<Core.Entities.BookLevel, BookLevelViewModel>();
        CreateMap<Core.Entities.BookLevel, Application.Responses.Books.BookLevel>();
        CreateMap<OrderTrade, OrderTradeViewModel>();

        CreateMap<InsertOrderTradeCommand, OrderTradeViewModel>();
        CreateMap<BookLevelCommand, BookLevelViewModel>();
    }
}