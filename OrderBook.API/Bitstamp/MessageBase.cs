using OrderBook.Core.Enumerations;
using System.Net.WebSockets;

namespace OrderBook.API.Bitstamp;

public class MessageBase
{
    public virtual MessageType Event {  get; set; }
}
