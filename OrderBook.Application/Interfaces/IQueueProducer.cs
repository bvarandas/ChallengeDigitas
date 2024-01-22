namespace OrderBook.Application.Interfaces;

public interface IQueueProducer
{
    Task PublishMessage(Core.Entities.OrderBook message);
}
