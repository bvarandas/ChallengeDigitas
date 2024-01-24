using OrderBook.Core.Enumerations;
using System.Net.WebSockets;

namespace OrderBook.Application.Responses;

public class MessageBase
{
    public virtual MessageType Event {  get; set; }
}
