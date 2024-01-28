using AutoMapper;
using OrderBook.Application.Commands;
using OrderBook.Application.Responses.Books;
using OrderBook.Application.ViewModel;
using OrderBook.Core.AggregateObjects;
using OrderBook.Core.Entities;

namespace OrderBook.Application.Automapper;
public class ViewModelToDomainMappingProfile : Profile
{
    public ViewModelToDomainMappingProfile()
    {
        CreateMap<OrderBookViewModel, InsertOrderBookCommand>();
        CreateMap<OrderBookViewModel, UpdateOrderBookCommand>();
        CreateMap<OrderBookRoot, InsertOrderBookCommand>();
        CreateMap<OrderBookRoot, UpdateOrderBookCommand>();

        CreateMap<OrderBook.Application.Responses.Books.BookLevel, BookLevelCommand>();
        CreateMap<OrderBook.Application.Responses.Books.OrderBook, UpdateOrderBookCommand>();
        CreateMap<OrderBook.Application.Responses.Books.OrderBook, InsertOrderBookCommand>();
        
        CreateMap<BookLevelCommand, OrderBook.Core.Entities.BookLevel>();
        CreateMap<UpdateOrderBookCommand, OrderBookRoot>();
        CreateMap<InsertOrderBookCommand, OrderBookRoot>();
        CreateMap<OrderTradeCommand, OrderTrade>()
            .ConstructUsing(c=> new OrderTrade(c.Ticker, c.QuantityRequested, c.TradeSide,null!,0));

        CreateMap<InsertOrderTradeCommand, OrderTrade>();
            //.ConstructUsing(c => new OrderBook.Core.Entities.OrderTrade(c.Ticker, c.QuantityRequested, c.TradeSide, c.Quotes, c.AmountShaved ));
    }
}