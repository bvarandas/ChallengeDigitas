using Common.Logging.Correlation;
using OrderBook.API.Bitstamp;
using OrderBook.API.Queue;
using OrderBook.Application.Interfaces;
using OrderBook.Core.Specs;
using System.Reflection;
namespace OrderBook.API.Configurations;
internal class NativeInjectorBoostrapper
{
    public static void RegisterServices(IServiceCollection services, IConfiguration config)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.Configure<QueueCommandSettings>(config.GetSection(nameof(QueueCommandSettings)));
        //services.Configure<QueueEventSettings>(config.GetSection(nameof(QueueEventSettings)));
        services.AddSingleton<IQueueProducer, QueueProducer>();
        services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

        //SignalR
        services.AddSignalR();

        // Asp .NET HttpContext dependency
        services.AddHttpContextAccessor();

        // Mediator
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        services.AddCors(options => options.AddPolicy("CorsPolicy", builderc =>
        {
            builderc
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials();
        }));

        //services.AddHostedService<QueueProducer>();
        services.AddHostedService<WorkerConsumeBitstamp>();
    }
}