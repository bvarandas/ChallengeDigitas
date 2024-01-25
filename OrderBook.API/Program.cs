using OrderBook.API.Configurations;
using OrderBook.Application.Commands;
using OrderBook.Application.Interfaces;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapPost("api/orderbook/trade", async(OrderTradeCommand command, IOrderBookService service, ILogger<Program> logger ) =>
{
    try
    {
        //await service.

        return Results.Accepted(null, command);
    }catch(Exception ex)
    {
        logger.LogError(ex, $"{ex.Message}");
        return Results.BadRequest(ex);
    }
    
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();
