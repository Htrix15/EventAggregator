using EventAggregator.Kassir.Infrastructure.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;


namespace EventAggregator.Kassir.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHostedService<MessageManagerBackgroundService>();
        return services;
    }
}

