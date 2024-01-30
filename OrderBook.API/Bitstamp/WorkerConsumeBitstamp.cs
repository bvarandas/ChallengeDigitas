using OrderBook.Application.Interfaces;
using Serilog.Events;
using Serilog;
using System.Reflection;
using System.Runtime.Loader;
using OrderBook.API.Communicator;
using OrderBook.API.Bitstamp.Client;
using OrderBook.Core.Enumerations;
using OrderBook.Core.Specs;
using Microsoft.Extensions.Options;
using OrderBook.Application.Responses.Responses.Requests;
using ChallengeCrf.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using OrderBook.Application;

namespace OrderBook.API.Bitstamp;
public class WorkerConsumeBitstamp : BackgroundService
{
    private readonly QueueCommandSettings _queueSettings;
    private readonly ILogger<WorkerConsumeBitstamp> _logger;
    private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);
    private readonly IQueueProducer _queueProducer;
    private readonly IOrderBookService _orderBookService;
    private readonly IServiceProvider _serviceProvider;
    private static TimeSpan _lastCycle;
    public WorkerConsumeBitstamp(IOptions<QueueCommandSettings> queueSettings, 
        ILogger<WorkerConsumeBitstamp> logger, 
        IQueueProducer queueProducer, 
        IOrderBookService orderBookService,
        IServiceProvider provider)
    {
        _logger = logger;
        _queueProducer = queueProducer;
        _queueSettings = queueSettings.Value;
        _orderBookService = orderBookService;
        _serviceProvider = provider;
        _lastCycle = DateTime.Now.TimeOfDay;
    }
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitLogging();

        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        AssemblyLoadContext.Default.Unloading += DefaultOnUnloading;

        Log.Debug("====================================");
        Log.Debug("              STARTING              ");
        Log.Debug("====================================");

        using (var communicator = new WebsocketCommunicator(new Uri("wss://ws.bitstamp.net")))
        {
            communicator.Name = "socket-01";

            using (var client = new WebsocketClient(communicator))
            {
                SubscribeToStreamsAsync(client);

                communicator.ReconnectionHappened.Subscribe(async type =>
                {
                    Log.Information($"Reconnection happened, type: {type.Type}, resubscribiging..");
                    await SendSubscriptionRequests(client);
                });

                await communicator.Start();

                ExitEvent.WaitOne();
            }
        }

        Log.Debug("====================================");
        Log.Debug("              STOPPING              ");
        Log.Debug("====================================");
        Log.CloseAndFlush();

        while (!stoppingToken.IsCancellationRequested)
        {
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(_queueSettings.Interval, stoppingToken);
        }
    }
    private static async Task SendSubscriptionRequests(WebsocketClient client)
    {
        client.Send(new SubscribeRequest("btcusd", Channel.OrderBook));
        client.Send(new SubscribeRequest("ethusd", Channel.OrderBook));
    }

    private async void SubscribeToStreamsAsync(WebsocketClient client)
    {
        

        client.Streams.ErrorStream.Subscribe(x =>
                        Log.Warning($"Error received, message: {x?.Message}"));

        client.Streams.SubscriptionSucceededStream.Subscribe(x =>
        {
            Log.Information($"Subscribed to {x?.Symbol} {x?.Channel}");
        });

        client.Streams.UnsubscriptionSucceededStream.Subscribe(x =>
        {
            Log.Information($"Unsubscribed from {x?.Symbol} {x?.Channel}");
        });

        client.Streams.OrderBookStream.Subscribe(x =>
        {
            Log.Information($"Order book L2 [{x.Symbol}]");
            Log.Information($"    {x.Data?.Asks.FirstOrDefault()?.Price} " +
                            $"{x.Data?.Asks.FirstOrDefault()?.Amount ?? 0} " +
                            $"{x.Data?.Asks.FirstOrDefault()?.Side} " +
                            $"({x.Data?.Asks?.Length})");
            Log.Information($"    {x.Data?.Bids.FirstOrDefault()?.Price} " +
                            $"{x.Data?.Bids.FirstOrDefault()?.Amount ?? 0} " +
                            $"{x.Data?.Bids.FirstOrDefault()?.Side} " +
                            $"({x.Data?.Bids?.Length})");
            
            x.Data.Ticker = x.Symbol;
            
            Array.Sort(x.Data.Asks, new AsksComparer());
            Array.Sort(x.Data.Bids, new BidsComparer());

            this.SendMessageClientAsync(x.Data);
            _queueProducer.PublishMessage(x.Data);

        });

        client.Streams.HeartbeatStream.Subscribe(x =>
                Log.Information($"Heartbeat received, product: {x?.Channel}, seq: {x?.Event}"));
    }

    private async Task SendMessageClientAsync(Application.Responses.Books.OrderBook orderBook)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<BrokerHub>>();

            if (orderBook.Ticker.StartsWith('b'))
                await hubContext.Clients.Group("CrudMessage").SendAsync("ReceiveMessageBtc", orderBook);
            else
                await hubContext.Clients.Group("CrudMessage").SendAsync("ReceiveMessageEth", orderBook);

            await _orderBookService.AddOrderBookCacheAsync(orderBook);
        }

        var now = DateTime.Now;
        if ((now.TimeOfDay - _lastCycle).TotalSeconds >= 2)
        {
            _lastCycle = now.TimeOfDay;
            using (var scope = _serviceProvider.CreateScope())
            {
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<BrokerHub>>();

                var orderbookData =_orderBookService.GetOrderBookDataCacheAsync(orderBook.Ticker);

                if (orderBook.Ticker.StartsWith('b'))
                    await hubContext.Clients.Group("CrudMessage").SendAsync("ReceiveMessageDataBtc", orderbookData.Result);
                else
                    await hubContext.Clients.Group("CrudMessage").SendAsync("ReceiveMessageDataEth", orderbookData.Result);
            }
        }
    }

    private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
    {
        Log.Warning("Exiting process");
        ExitEvent.Set();
    }

    private static void InitLogging()
    {
        var executingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
        var logPath = Path.Combine(executingDir, "logs", "verbose.log");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .WriteTo.Console(LogEventLevel.Verbose)
            .CreateLogger();
    }
    private static void DefaultOnUnloading(AssemblyLoadContext assemblyLoadContext)
    {
        Log.Warning("Unloading process");
        ExitEvent.Set();
    }
}
