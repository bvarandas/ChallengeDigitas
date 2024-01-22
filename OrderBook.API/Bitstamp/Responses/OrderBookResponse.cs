using OrderBook.API.Json;
using Newtonsoft.Json.Linq;
using System.Reactive.Subjects;
using OrderBook.Core.Enumerations;

namespace OrderBook.API.Bitstamp.Responses;
public class OrderBookResponse : ResponseBase
{
    public override MessageType Event => MessageType.OrderBook;

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