using Newtonsoft.Json.Linq;
using System.Reactive.Subjects;
using OrderBook.Application.Json;
namespace OrderBook.Application.Responses;

public class HeartbeatResponse: ResponseBase
{
    internal static bool TryHandle(JObject response, ISubject<HeartbeatResponse> subject)
    {
        //if (response?["channel"].Value<string>() != "heartbeat") return false;
        if (response != null && (bool)!response?["channel"].Value<string>().StartsWith("heartbeat"))
            return false;

        var parsed = response.ToObject<HeartbeatResponse>(BitstampJsonSerializer.Serializer);
        subject.OnNext(parsed);
        return true;
    }
}
