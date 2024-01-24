using Newtonsoft.Json;
namespace OrderBook.Application.Requests;
public abstract class RequestBase
{
    [JsonProperty("event")]
    public virtual string Event { get; set; }

    [JsonProperty("data")] public virtual RequestData RequestData { get; set; } = null!;
}
