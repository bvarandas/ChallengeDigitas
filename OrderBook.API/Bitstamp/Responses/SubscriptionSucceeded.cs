using OrderBook.API.Json;
using Newtonsoft.Json.Linq;
using System.Reactive.Subjects;
using OrderBook.Core.Enumerations;
namespace OrderBook.API.Bitstamp.Responses;

public class SubscriptionSucceeded : ResponseBase
{
    public override MessageType Event => MessageType.Subscribe;

    internal static bool TryHandle(JObject response, ISubject<SubscriptionSucceeded> subject)
    {
        var eventName = response?["event"];
        if (eventName == null || eventName.Value<string>() != "bts:subscription_succeeded")
            return false;

        var parsed = response.ToObject<SubscriptionSucceeded>(BitstampJsonSerializer.Serializer);

        var channelName = response["channel"];
        if (parsed != null && channelName != null)
            parsed.Symbol = channelName.Value<string>().Split('_').LastOrDefault();

        subject.OnNext(parsed);
        return true;
    }
}
