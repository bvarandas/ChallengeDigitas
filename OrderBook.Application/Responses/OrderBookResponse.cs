using Newtonsoft.Json.Linq;
using System.Reactive.Subjects;
using OrderBook.Core.Enumerations;
using OrderBook.Application.Responses.Json;
using OrderBook.Application.Responses;

namespace OrderBook.API.Bitstamp.Responses;
public class OrderBookResponse : ResponseBase
{
    public override MessageType Event => MessageType.OrderBook;

    public Application.Responses.Books.OrderBook Data { get; set; } = null!;

    public static bool TryHandle(JObject response, ISubject<OrderBookResponse> subject)
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