using Microsoft.Extensions.DependencyInjection;
namespace OrderBook.Application.Automapper;
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
