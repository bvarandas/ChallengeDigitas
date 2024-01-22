using Microsoft.Extensions.Options;
using OrderBook.Application.Interfaces;
using OrderBook.Core.Extensions;
using OrderBook.Core.Specs;
using RabbitMQ.Client;

namespace OrderBook.API.Queue;

public class QueueProducer : BackgroundService, IQueueProducer
{
    private readonly QueueCommandSettings _queueSettings;
    private readonly ConnectionFactory _factory = null!;
    private readonly IModel _channel = null!;
    private readonly IConnection _connection = null!;
    private readonly ILogger<QueueProducer> _logger;

    public QueueProducer(IOptions<QueueCommandSettings> queueSettings, ILogger<QueueProducer> logger)
    {

        _logger = logger;
        _queueSettings = queueSettings.Value;
        try
        {
            _factory = new ConnectionFactory { HostName = _queueSettings.HostName, Port = 5672 };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: "amq.direct",
                type: _queueSettings.ExchangeType,
                durable: true,
                autoDelete: false);

            _channel.QueueDeclare(
                queue: _queueSettings.QueueNameOrderBook,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public Task PublishMessage(Core.Entities.OrderBook message)
    {
        try
        {
            _logger.LogInformation($"QueueProducer - Enviando mensagem nova {message.Id}");

            var body = message.SerializeToByteArrayProtobuf();

            _channel.BasicPublish(
                exchange: "",
                routingKey: _queueSettings.QueueNameOrderBook,
                basicProperties: null,
                body: body);

            _logger.LogInformation("QueueProducer - Mensagem enviada");

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}");
            return Task.FromException(ex);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(_queueSettings.Interval, stoppingToken);
        }
    }
}
