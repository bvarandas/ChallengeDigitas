using Newtonsoft.Json;
namespace OrderBook.Application.Requests;
public class RequestData
{
    [JsonProperty("channel")] public string Channel { get; set; } = string.Empty;
}
