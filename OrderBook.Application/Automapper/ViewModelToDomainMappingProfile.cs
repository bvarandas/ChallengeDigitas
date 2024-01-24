using AutoMapper;
using OrderBook.Application.Commands;
using OrderBook.Application.ViewModel;

namespace OrderBook.Application.Automapper;
public class ViewModelToDomainMappingProfile : Profile
{
    public ViewModelToDomainMappingProfile()
    {
        CreateMap<OrderBookViewModel, InsertOrderBookCommand>();
                //.ConstructUsing(c => new InsertOrderBookCommand(c.Description, c.Amount, c.Entry, c.Date));
        CreateMap<OrderBookViewModel, UpdateOrderBookCommand>();
        //.ConstructUsing(c => new UpdateOrderBookCommand(c.CashFlowId, c.Description, c.Amount, c.Entry, c.Date));
        CreateMap<OrderBook.Core.Entities.OrderBook, InsertOrderBookCommand>();
        //.ConstructUsing(c => new InsertOrderBookCommand(c.Description, c.Amount, c.Entry, c.Date));
        CreateMap<OrderBook.Core.Entities.OrderBook, UpdateOrderBookCommand>();
            //.ConstructUsing(c => new UpdateOrderBookCommand(c.CashFlowId, c.Description, c.Amount, c.Entry, c.Date));
    }
}
