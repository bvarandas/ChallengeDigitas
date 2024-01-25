using AutoMapper;
using OrderBook.Application.Commands;
using OrderBook.Application.ViewModel;

namespace OrderBook.Application.Automapper;
public class ViewModelToDomainMappingProfile : Profile
{
    public ViewModelToDomainMappingProfile()
    {
        CreateMap<OrderBookViewModel, InsertOrderBookCommand>();
        CreateMap<OrderBookViewModel, UpdateOrderBookCommand>();
        CreateMap<OrderBook.Core.Entities.OrderBook, InsertOrderBookCommand>();
        CreateMap<OrderBook.Core.Entities.OrderBook, UpdateOrderBookCommand>();

        CreateMap<OrderBook.Application.Responses.Books.BookLevel, BookLevelCommand>();
        CreateMap<OrderBook.Application.Responses.Books.OrderBook, UpdateOrderBookCommand>();
        CreateMap<OrderBook.Application.Responses.Books.OrderBook, InsertOrderBookCommand>();
        
        CreateMap<BookLevelCommand, OrderBook.Core.Entities.BookLevel>();
        CreateMap<UpdateOrderBookCommand, OrderBook.Core.Entities.OrderBook>();
        CreateMap<InsertOrderBookCommand, OrderBook.Core.Entities.OrderBook>();

    }
}
