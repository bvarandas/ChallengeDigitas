using OrderBook.Application.Json;
using OrderBook.Application.Messages;
using Newtonsoft.Json.Linq;
using System.Reactive.Subjects;

namespace OrderBook.Application.Responses;

/// <summary>
/// Full order book response L2 - first 100 levels on both sides
/// </summary>
public class OrderBookResponse : ResponseBase
{
    /// <summary>
    /// Order book event type
    /// </summary>
    public override MessageType Event => MessageType.OrderBook;

    /// <summary>
    /// Order book data
    /// </summary>
    public Books.OrderBook Data { get; set; } = null!;

    internal static bool TryHandle(JObject response, ISubject<OrderBookResponse> subject)
    {
        var channelName = response?["channel"];
        if (channelName == null || !channelName.Value<string>().StartsWith("order_book"))
            return false;

        var parsed = response?.ToObject<OrderBookResponse>(BitstampJsonSerializer.Serializer);
        if (parsed != null)
        {
            parsed.Symbol = channelName.Value<string>().Split('_').LastOrDefault();
            subject.OnNext(parsed);
        }

        return true;
    }
}