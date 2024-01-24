using Newtonsoft.Json.Linq;
using System.Reactive.Subjects;
using OrderBook.API.Bitstamp.Responses;
using OrderBook.Core.Enumerations;
using OrderBook.Application.Responses;
using OrderBook.Application.Responses.Json;

namespace OrderBook.API.Responses;

public class UnsubscriptionSucceeded : ResponseBase
{
    public override MessageType Event => MessageType.Unsubscribe;

    public static bool TryHandle(JObject response, ISubject<UnsubscriptionSucceeded> subject)
    {
        var eventName = response?["event"];
        if (eventName == null || eventName.Value<string>() != "bts:unsubscription_succeeded")
            return false;

        var parsed = response.ToObject<UnsubscriptionSucceeded>(BitstampJsonSerializer.Serializer);

        var channelName = response["channel"];
        if (parsed != null && channelName != null)
            parsed.Symbol = channelName.Value<string>().Split('_').LastOrDefault();

        subject.OnNext(parsed);
        return true;
    }
}
