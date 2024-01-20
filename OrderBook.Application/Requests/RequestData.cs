using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Application.Requests;

public class RequestData
{
    [JsonProperty("channel")] public string Channel { get; set; } = string.Empty;
}
