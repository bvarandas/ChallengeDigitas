﻿// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Common.Logging;
using Common.Logging.Correlation;
using OrderBook.Worker.Workers;
using Microsoft.Extensions.DependencyInjection;
using OrderBook.Core.Specs;
using MongoFramework;
using System.Reflection;
using FluentResults;
using MediatR;
using OrderBook.Infrastructure.Data;
using OrderBook.Core.Repositories;
using OrderBook.Infrastructure.Repositories;
using OrderBook.Application.Commands;
using OrderBook.Application.Handlers;
using OrderBook.Application.Automapper;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder =>
        {
            builder.Sources.Clear();
            builder.AddConfiguration(config);

        })
        .UseSerilog(Logging.ConfigureLogger)
        .ConfigureServices(services =>
        {
            services.Configure<QueueCommandSettings>(config.GetSection(nameof(QueueCommandSettings)));

            //services.AddSingleton<IWorkerProducer, WorkerProducer>();
            //services.AddSingleton<IWorkerConsumer, WorkerConsumer>();

            

            //services.Configure<CashFlowSettings>(config.GetSection("CashFlowStoreDatabase"));
            services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

            //services.AddTransient<IMongoDbConnection>((provider) =>
            //{
            //    var urlMongo = new MongoDB.Driver.MongoUrl("mongodb://root:example@mongo:27017/challengeCrf?authSource=admin");

            //    return MongoDbConnection.FromUrl(urlMongo);
            //});

            services.AddAutoMapperSetup();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            });

            // Domain - Commands
            services.AddSingleton<IRequestHandler<InsertOrderBookCommand, Result<bool>>, InsertOrderBookCommandHandler>();
            
            // Infra - Data
            services.AddSingleton<IOrderBookRepository, OrderBookRepository>();
            services.AddSingleton<IOrderTradeRepository, OrderTradeRepository>();

            services.AddSingleton< IOrderBookContext, OrderBookContext >();
            services.AddSingleton<IOrderTradeContext, OrderTradeContext>();
            services.AddHostedService<WorkerConsumer>();
        }).Build();

await host
.RunAsync();