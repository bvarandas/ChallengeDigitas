using ChallengeCrf.Api.Hubs;
using Common.Logging;
using OrderBook.API.Configurations;
using OrderBook.Application.Commands;
using OrderBook.Application.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
    //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

NativeInjectorBoostrapper.RegisterServices(builder.Services, config);
builder.Host.UseSerilog(Logging.ConfigureLogger);

var app = builder.Build();
//app.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapPost("api/orderbook/trade", async (OrderTradeCommand command, IOrderBookService service, ILogger<Program> logger) =>
{
    try
    {
        //throw new Exception("Error de propósito");
        var resultTrade = await service.SendOrderTradeAsync(command);
        return Results.Accepted(null, resultTrade);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"{ex.Message}");
        return Results.BadRequest(ex);
    }
});

app.UseCors("CorsPolicy");
app.MapHub<BrokerHub>("/hubs/brokerhub");

app.Run();

public partial class Program { }