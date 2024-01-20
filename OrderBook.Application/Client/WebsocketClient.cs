using OrderBook.Application.Communicator;
using OrderBook.Application.Json;
using OrderBook.Application.Logging;
using OrderBook.Application.Responses;
using Newtonsoft.Json.Linq;
using Websocket.Client;
using OrderBook.Application.Requests;

namespace OrderBook.Application.Client;

public class WebsocketClient : IDisposable
{
    private static readonly ILog Log = LogProvider.GetCurrentClassLogger();
    private readonly ICommunicator _communicator;
    private readonly IDisposable _messageReceivedSubscription;
    public BitstampClientStreams Streams { get; } = new BitstampClientStreams();

    public WebsocketClient(ICommunicator communicator)
    {
        _communicator = communicator;
        _messageReceivedSubscription = _communicator.MessageReceived.Subscribe();
    }
    private bool HandleRawMessage(string msg)
    {
        // ********************
        // ADD RAW HANDLERS BELOW
        // ********************

        return false;
    }

    private void HandleMessage(ResponseMessage message)
    {
        try
        {
            bool handled;
            var messageSafe = (message.Text ?? string.Empty).Trim();

            if (messageSafe.StartsWith("{"))
            {
                handled = HandleObjectMessage(messageSafe);
                if (handled) return;
            }

            handled = HandleRawMessage(messageSafe);
            if (handled) return;

            Log.Warn(L($"Unhandled response:  '{messageSafe}'"));
        }
        catch (Exception e)
        {
            Log.Error(e, L("Exception while receiving message"));
        }
    }

    public void Send<T>(T request) where T : RequestBase
    {
        try
        {
            var serialized =
                BitstampJsonSerializer.Serialize(request);

            _communicator.Send(serialized);
        }
        catch (Exception e)
        {
            Log.Error(e, L($"Exception while sending message '{request}'. Error: {e.Message}"));
            throw;
        }
    }

    private string L(string msg)
    {
        return $"[BITSTAMP WEBSOCKET CLIENT] {msg}";
    }

    private bool HandleObjectMessage(string msg)
    {
        var response = BitstampJsonSerializer.Deserialize<JObject>(msg);

        return
            SubscriptionSucceeded.TryHandle(response, Streams.SubscriptionSucceededSubject) ||
            UnsubscriptionSucceeded.TryHandle(response, Streams.UnsubscriptionSucceededSubject) ||
            //OrderBookSnapshotResponse.TryHandle(response, Streams.OrderBookSnapshotSubject) ||
            //Ticker.TryHandle(response, Streams.TickerSubject) ||
            OrderBookResponse.TryHandle(response, Streams.OrderBookSubject) || false;
            
    }
    public void Dispose()
    {
        _messageReceivedSubscription?.Dispose();
    }
}
