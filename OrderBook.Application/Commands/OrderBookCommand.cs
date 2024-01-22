using FluentResults;
using MediatR;
namespace OrderBook.Application.Commands;
public class InsertOrderBookCommand : Core.Entities.OrderBook, IRequest<Result<bool>> { }
public class UpdateOrderBookCommand : Core.Entities.OrderBook, IRequest<Result<bool>> { }