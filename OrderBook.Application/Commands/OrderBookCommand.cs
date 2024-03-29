﻿using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OrderBook.Application.Validations;
using OrderBook.Application.ViewModel;
using OrderBook.Core.Enumerations;
namespace OrderBook.Application.Commands;
public abstract class OrderBookCommand
{
    public string Ticker { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DateTime Microtimestamp { get; set; }
    public BookLevelCommand[] Bids { get; set; } = null!;
    public BookLevelCommand[] Asks { get; set; } = null!;
    public abstract ValidationResult Validation();
    public ValidationResult ValidationResult { get; set; } = null!;
}
public class BookLevelCommand
{
    public OrderBookSide Side { get; set; }
    public double Price { get; set; }
    public double Amount { get; set; }
    public long OrderId { get; set; }
}
public class InsertOrderBookCommand : OrderBookCommand, IRequest<Result<bool>> 
{
    public InsertOrderBookCommand(string ticker, DateTime timestamp, DateTime microtimestamp, BookLevelCommand[] bids, BookLevelCommand[] asks)
    {
        Ticker = ticker;
        Timestamp = timestamp;
        Microtimestamp = microtimestamp;
        Bids = bids;
        Asks = asks;
    }

    public override ValidationResult Validation()
    {
        ValidationResult = new InsertOrderBookCommandValidation().Validate(this);
        return ValidationResult;
    }
}
public class UpdateOrderBookCommand : OrderBookCommand, IRequest<Result<bool>> 
{
    public UpdateOrderBookCommand(string ticker, DateTime timestamp, DateTime microtimestamp, BookLevelCommand[] bids, BookLevelCommand[] asks)
    {
        Ticker = ticker;
        Timestamp = timestamp;
        Microtimestamp = microtimestamp;
        Bids = bids;
        Asks = asks;
    }

    public override ValidationResult Validation()
    {
        ValidationResult = new UpdateOrderBookCommandValidation().Validate(this);
        return ValidationResult;
    }
}