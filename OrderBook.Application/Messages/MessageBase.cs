using System.Net.WebSockets;

namespace OrderBook.Application.Messages;

public class MessageBase
{
    public virtual MessageType Event {  get; set; }
}
