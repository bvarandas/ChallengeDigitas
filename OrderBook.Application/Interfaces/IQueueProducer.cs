namespace OrderBook.Application.Interfaces;
public interface IQueueProducer
{
    Task PublishMessage( Responses.Books.OrderBook message);
}
