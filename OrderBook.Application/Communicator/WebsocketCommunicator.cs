using System.Net.WebSockets;
using Websocket.Client;

namespace OrderBook.Application.Communicator;

public class WebsocketCommunicator : WebsocketClient, ICommunicator
{
    public WebsocketCommunicator(Uri url, Func<ClientWebSocket> clientFactory = null) 
        : base(url, clientFactory)
    {
    }
}


public interface ICommunicator : IWebsocketClient
{
}