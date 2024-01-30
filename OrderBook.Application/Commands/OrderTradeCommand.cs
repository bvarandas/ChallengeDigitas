using FluentResults;
using FluentValidation.Results;
using MediatR;
using OrderBook.Application.Validations;
using OrderBook.Core.Enumerations;
namespace OrderBook.Application.Commands;
public abstract class OrderTradeCommand
{
    public string Ticker { get; set; } = null!;
    public double QuantityRequested { get; set; }
    public TradeSide TradeSide { get; set; }
    public abstract ValidationResult Validation();
    public ValidationResult ValidationResult { get; set; } = null!;
    public OrderTradeCommand() { }
    public OrderTradeCommand(string ticker, double quantityRequested, TradeSide tradeSide)
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
    public double TotalPriceShaved {  get; set; }
    public InsertOrderTradeCommand(string id,string ticker, double quantityRequested, TradeSide tradeSide, IList<BookLevelCommand> quotes, double amountShaved, double totalPriceShaved)
    {
        Id = id;
        Ticker = ticker;
        QuantityRequested = quantityRequested;
        TradeSide = tradeSide;
        Quotes = quotes;
        AmountShaved = amountShaved;
        TotalPriceShaved = totalPriceShaved;
    }

    public override ValidationResult Validation()
    {
        ValidationResult = new InsertOrderTradeCommandValidation().Validate(this);
        return ValidationResult;
    }
}