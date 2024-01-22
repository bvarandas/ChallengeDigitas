using OrderBook.API.Bitstamp.Responses;
using OrderBook.API.Responses;
using System.Reactive.Linq;
using System.Reactive.Subjects;
namespace OrderBook.API.Client;
public class BitstampClientStreams
{
    internal readonly Subject<ErrorResponse> ErrorSubject = new Subject<ErrorResponse>();
    internal readonly Subject<HeartbeatResponse> HeartbeatSubject = new Subject<HeartbeatResponse>();
    internal readonly Subject<OrderBookResponse> OrderBookSubject = new Subject<OrderBookResponse>();
    internal readonly Subject<Ticker> TickerSubject = new Subject<Ticker>();
    internal readonly Subject<SubscriptionSucceeded> SubscriptionSucceededSubject = new Subject<SubscriptionSucceeded>();
    internal readonly Subject<UnsubscriptionSucceeded> UnsubscriptionSucceededSubject = new Subject<UnsubscriptionSucceeded>();

    // PUBLIC

    /// <summary>
    /// Server errors stream.
    /// Error messages: Most failure cases will cause an error message to be emitted.
    /// This can be helpful for implementing a client or debugging issues.
    /// </summary>
    public IObservable<ErrorResponse> ErrorStream => ErrorSubject.AsObservable();

    /// <summary>
    /// Response stream to every ping request
    /// </summary>
    public IObservable<HeartbeatResponse> HeartbeatStream => HeartbeatSubject.AsObservable();

    /// <summary>
    /// Order book stream L2 - first 100 levels on both sides
    /// </summary>
    public IObservable<OrderBookResponse> OrderBookStream => OrderBookSubject.AsObservable();
    
    /// <summary>
    /// Executed trades
    /// </summary>
    public IObservable<Ticker> TickerStream => TickerSubject.AsObservable();

    /// <summary>
    /// Subscription stream
    /// </summary>
    public IObservable<SubscriptionSucceeded> SubscriptionSucceededStream => SubscriptionSucceededSubject.AsObservable();

    /// <summary>
    /// Unsubscribe stream
    /// </summary>
    public IObservable<UnsubscriptionSucceeded> UnsubscriptionSucceededStream => UnsubscriptionSucceededSubject.AsObservable();
}
