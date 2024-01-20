using OrderBook.Application.Messages;

namespace OrderBook.Application.Responses;

public abstract class ResponseBase : MessageBase
{
    public string Channel { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
}
