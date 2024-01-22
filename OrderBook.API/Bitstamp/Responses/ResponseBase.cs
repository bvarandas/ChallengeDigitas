namespace OrderBook.API.Bitstamp.Responses;
public abstract class ResponseBase : MessageBase
{
    public string Channel { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
}