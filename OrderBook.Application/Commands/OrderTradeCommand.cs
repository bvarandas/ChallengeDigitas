using FluentResults;
using MediatR;
using OrderBook.Core.Enumerations;
using OrderBook.Core.ValuesObject;
namespace OrderBook.Application.Commands;
public class OrderTradeCommand
{
    public Ticker Ticker { get; set; } = null!;
    public double QuantityRequested { get; set; }
    public TradeSide TradeSide { get; set; }

    public OrderTradeCommand() { }

    public OrderTradeCommand(Ticker ticker, double quantityRequested, TradeSide tradeSide)
    {
        Ticker = ticker;
        QuantityRequested = quantityRequested;
        TradeSide = tradeSide;
    }
}

public class InsertOrderTradeCommand : OrderTradeCommand, IRequest<Result<bool>>
{
    public string Id { get; set; }
    public IList<BookLevelCommand> Quotes { get; set; }
    public double AmountShaved { get; set; }

    public InsertOrderTradeCommand(string id,Ticker ticker, double quantityRequested, TradeSide tradeSide, IList<BookLevelCommand> quotes, double amountShaved)
    {
        Id = id;
        Ticker = ticker;
        QuantityRequested = quantityRequested;
        TradeSide = tradeSide;
        Quotes = quotes;
        AmountShaved = amountShaved;
    }
}