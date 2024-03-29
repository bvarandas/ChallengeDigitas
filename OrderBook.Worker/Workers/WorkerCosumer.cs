using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using OrderBook.Application.Interfaces;
using AutoMapper;
using OrderBook.Application.Commands;
using OrderBook.Core.Extensions;
using Microsoft.Extensions.Hosting;
using OrderBook.Core.Specs;
using Microsoft.Extensions.Logging;
namespace OrderBook.Worker.Workers;
public class WorkerConsumer : BackgroundService, IWorkerConsumer
{
    private readonly IMediator _mediator;
    private readonly ILogger<WorkerConsumer> _logger;
    private readonly QueueCommandSettings _queueSettings;
    private readonly ConnectionFactory _factory;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IMapper _mapper;
    public WorkerConsumer(IOptions<QueueCommandSettings> queueSettings, ILogger<WorkerConsumer> logger, IMapper mapper, IMediator mediator)
    {
        _logger = logger;
        _queueSettings = queueSettings.Value;
        
        _mapper = mapper;

        _logger.LogInformation($"O hostname � {_queueSettings.HostName}");
        
        _factory = new ConnectionFactory()
        {
            HostName = _queueSettings.HostName,
            Port = _queueSettings.Port,
        };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.QueueDeclare(_queueSettings.QueueNameOrderBook,
            durable: true, 
            exclusive: false, 
            autoDelete: false);

        _mediator = mediator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Aguardando mensagens Command...");

        var consumerOrderBook = new EventingBasicConsumer(_channel);
        consumerOrderBook.Received += ConsumerOrderBook_Received;

        _channel.BasicConsume(queue: _queueSettings.QueueNameOrderBook, autoAck: false, consumer: consumerOrderBook);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_queueSettings.Interval, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _channel.Close();
    }

    private async void ConsumerOrderBook_Received(object? sender, BasicDeliverEventArgs e)
    {
        try
        {
            _logger.LogInformation("Chegou nova mensagem no Worker Consumer");
            var message = e.Body.ToArray().DeserializeFromByteArrayProtobuf< Application.Responses.Books.OrderBook>();
            _logger.LogInformation("Conseguiu Deserializar a mensagem");
            _logger.LogInformation($"{message.Ticker}");

            var commandInsert = _mapper.Map<Application.Responses.Books.OrderBook, InsertOrderBookCommand>(message);
            _mediator.Send(commandInsert);

            _channel.BasicAck(e.DeliveryTag, true);

        }catch (Exception ex) 
        {
            _logger.LogError(ex.Message, ex);
            //_channel.BasicNack(e.DeliveryTag, false, true);
            _channel.BasicAck(e.DeliveryTag, true);
        }
    }
}