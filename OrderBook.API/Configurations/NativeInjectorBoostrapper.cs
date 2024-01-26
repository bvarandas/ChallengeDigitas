using Common.Logging.Correlation;
using OrderBook.API.Bitstamp;
using OrderBook.API.Queue;
using OrderBook.Application;
using OrderBook.Application.Interfaces;
using OrderBook.Core.Specs;
using System.Reflection;
using OrderBook.Application.Automapper;
using OrderBook.Core.Repositories;
using OrderBook.Infrastructure.Repositories;
using OrderBook.Infrastructure.Data;
using FluentResults;
using MediatR;
using OrderBook.Application.Commands;
using OrderBook.Application.Handlers;

namespace OrderBook.API.Configurations;
internal class NativeInjectorBoostrapper
{
    public static void RegisterServices(IServiceCollection services, IConfiguration config)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.Configure<QueueCommandSettings>(config.GetSection(nameof(QueueCommandSettings)));
        
        services.AddSingleton<IQueueProducer, QueueProducer>();
        services.AddSingleton<ICorrelationIdGenerator, CorrelationIdGenerator>();
        services.AddSingleton<IOrderBookService, OrderBookService>();
        //services.AddSingleton<IOrderTradeService, OrderBookService>();
        services.AddSingleton<IOrderBookRepository, OrderBookRepository>();
        services.AddSingleton<IOrderTradeRepository, OrderTradeRepository>();
        services.AddSingleton<IOrderBookContext, OrderBookContext>();
        services.AddSingleton<IOrderTradeContext, OrderTradeContext>();
        //SignalR
        services.AddSignalR();

        // Asp .NET HttpContext dependency
        services.AddHttpContextAccessor();

        // Mediator
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        // Domain - Commands
        services.AddSingleton<IRequestHandler<InsertOrderTradeCommand, Result<bool>>, InsertOrderTradeCommandHandler>();

        services.AddAutoMapperSetup();

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