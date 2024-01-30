using FluentValidation;
using OrderBook.Application.Commands;
namespace OrderBook.Application.Validations;
public abstract class OrderBookValidation<T> : AbstractValidator<T> where T : OrderBookCommand
{
    protected void ValidateTicker()
    {
        RuleFor(c => c.Ticker)
            .NotEmpty().WithMessage("É necessário inserir o Ticker (Symbol)!")
            .Length(3, 150).WithMessage("É necessário inserir ao menos 3 caracteres no Ticker!");
    }

    protected void ValidateBidsNull()
    {
        RuleFor(c => c.Bids)
            .NotNull()
            .WithMessage("É necessário inserir ao menos um Bid válido!");
    }

    protected void ValidateBids()
    {
        RuleFor(c => c.Bids.Length)
            .NotEqual(0)
            .WithMessage("É necessário inserir ao menos um Bid válido!");
    }

    protected void ValidateAsksNull()
    {
        RuleFor(c => c.Asks)
            .NotNull()
            .WithMessage("É necessário inserir ao menos um Asks válido!");
    }

    protected void ValidateAsksLength()
    {
        RuleFor(c => c.Asks.Length)
            .NotEqual(0)
            .WithMessage("É necessário inserir ao menos um Asks válido!");
    }
}
public abstract class OrderBookTradeValidation<T> : AbstractValidator<T> where T : OrderTradeCommand
{
    protected void ValidateTicker()
    {
        RuleFor(c => c.Ticker)
            .NotEmpty().WithMessage("É necessário inserir o Ticker (Symbol)!")
            .Length(3, 150).WithMessage("É necessário inserir ao menos 3 caracteres no Ticker!");
    }

    protected void ValidateQuantityRequest()
    {
        RuleFor(c => c.QuantityRequested)
            .NotEqual(0)
            .WithMessage("É necessário inserir um QuantityRequested válido!")
            .LessThan(0)
            .WithMessage("É necessário inserir um QuantityRequested positivo!");
    }

    protected void ValidateTradeSide()
    {
        RuleFor(c => c.TradeSide)
            .Null()
            .WithMessage("É necessário inserir ao menos um TradeSide válido!")
            .IsInEnum()
            .WithMessage("É necessário inserir ao menos um TradeSide válido!");
    }

    protected void ValidateTradeSideIsNull()
    {
        RuleFor(c => c.TradeSide)
            .NotNull()
            .WithMessage("É necessário inserir o TradeSide válido!");
    }
}
public class InsertOrderBookCommandValidation : OrderBookValidation<InsertOrderBookCommand>
{
    public InsertOrderBookCommandValidation() 
    {
        ValidateTicker();
        ValidateBidsNull();
        ValidateBids();
        ValidateAsksNull();
        ValidateAsksLength();
    }
}

public class UpdateOrderBookCommandValidation : OrderBookValidation<UpdateOrderBookCommand>
{
    public UpdateOrderBookCommandValidation()
    {
        ValidateTicker();
        ValidateBidsNull();
        ValidateBids();
        ValidateAsksNull();
        ValidateAsksLength();
    }
}

public class InsertOrderTradeCommandValidation : OrderBookTradeValidation<InsertOrderTradeCommand>
{
    public InsertOrderTradeCommandValidation()
    {
        ValidateTicker();
        ValidateQuantityRequest();
        ValidateTradeSide();
        ValidateTradeSideIsNull();
    }
}