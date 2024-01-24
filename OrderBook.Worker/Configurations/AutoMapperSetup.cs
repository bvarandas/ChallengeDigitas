using Microsoft.Extensions.DependencyInjection;
using OrderBook.Application.Automapper;
namespace OrderBook.Queue.Worker.Configurations;
public static class AutoMapperSetup
{
    public static void AddAutoMapperSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentException(nameof(services));

        var mapper = AutoMapperConfig
            .RegisterMappings()
            .CreateMapper();

        services.AddSingleton(mapper);
    }
}
