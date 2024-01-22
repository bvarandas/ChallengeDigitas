//using System.Reflection;
//using System.Runtime.Loader;
//using OrderBook.Application.Channels;
//using OrderBook.Application.Client;
//using OrderBook.Application.Communicator;
//using OrderBook.Application.Requests;
//using Serilog;
//using Serilog.Events;

//namespace OrderBook.Application.App;
//internal class Program
//{
//    private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);

//    private static readonly string API_KEY = "your api key";
//    private static readonly string API_SECRET = "";

//    private static async Task Main(string[] args)
//    {
//        InitLogging();

//        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
//        AssemblyLoadContext.Default.Unloading += DefaultOnUnloading;
//        Console.CancelKeyPress += Console_CancelKeyPress;

//        Console.WriteLine("|=======================|");
//        Console.WriteLine("|    BITSTAMP CLIENT    |");
//        Console.WriteLine("|=======================|");
//        Console.WriteLine();

//        Log.Debug("====================================");
//        Log.Debug("              STARTING              ");
//        Log.Debug("====================================");

//        using (var communicator = new WebsocketCommunicator(new Uri("wss://ws.bitstamp.net")))
//        {
//            communicator.Name = "socket-01";

//            using (var client = new WebsocketClient(communicator))
//            {
//                SubscribeToStreams(client);

//                communicator.ReconnectionHappened.Subscribe(async type => 
//                {
//                    Log.Information($"Reconnection happened, type: {type.Type}, resubscribiging..");
//                    await SendSubscriptionRequests(client);
//                });

//                await communicator.Start();

//                ExitEvent.WaitOne();
//            }
//        }

//        Log.Debug("====================================");
//        Log.Debug("              STOPPING              ");
//        Log.Debug("====================================");
//        Log.CloseAndFlush();
//    }

//    private static async Task SendSubscriptionRequests(WebsocketClient client)
//    {
//        client.Send(new SubscribeRequest("btcusd", Channel.OrderBook));
//        client.Send(new SubscribeRequest("ethusd", Channel.OrderBook));
//    }

//    private static void SubscribeToStreams(WebsocketClient client)
//    {
//        client.Streams.ErrorStream.Subscribe(x =>
//                        Log.Warning($"Error received, message: {x?.Message}"));

//        client.Streams.SubscriptionSucceededStream.Subscribe(x =>
//        {
//            Log.Information($"Subscribed to {x?.Symbol} {x?.Channel}");
//        });

//        client.Streams.UnsubscriptionSucceededStream.Subscribe(x =>
//        {
//            Log.Information($"Unsubscribed from {x?.Symbol} {x?.Channel}");
//        });

//        client.Streams.OrderBookStream.Subscribe(x =>
//        {
//            Log.Information($"Order book L2 [{x.Symbol}]");
//            Log.Information($"    {x.Data?.Asks.FirstOrDefault()?.Price} " +
//                            $"{x.Data?.Asks.FirstOrDefault()?.Amount ?? 0} " +
//                            $"{x.Data?.Asks.FirstOrDefault()?.Side} " +
//                            $"({x.Data?.Asks?.Length})");
//            Log.Information($"    {x.Data?.Bids.FirstOrDefault()?.Price} " +
//                            $"{x.Data?.Bids.FirstOrDefault()?.Amount ?? 0} " +
//                            $"{x.Data?.Bids.FirstOrDefault()?.Side} " +
//                            $"({x.Data?.Bids?.Length})");
//        });

//        client.Streams.HeartbeatStream.Subscribe(x =>
//                Log.Information($"Heartbeat received, product: {x?.Channel}, seq: {x?.Event}"));
//    }

//    private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
//    {
//        Log.Warning("Canceling process");
//        e.Cancel = true;
//        ExitEvent.Set();
//    }

//    private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
//    {
//        Log.Warning("Exiting process");
//        ExitEvent.Set();
//    }

//    private static void InitLogging()
//    {
//        var executingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
//        var logPath = Path.Combine(executingDir, "logs", "verbose.log");
//        Log.Logger = new LoggerConfiguration()
//            .MinimumLevel.Verbose()
//            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
//            .WriteTo.ColoredConsole(LogEventLevel.Verbose)
//            .CreateLogger();
//    }
//    private static void DefaultOnUnloading(AssemblyLoadContext assemblyLoadContext)
//    {
//        Log.Warning("Unloading process");
//        ExitEvent.Set();
//    }
//}