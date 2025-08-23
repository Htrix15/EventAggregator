using EventAggregator.Shared.ExternalServices.Services;
using EventAggregator.Shared.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventAggregator.Shared.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IExternalServiceHealthChecker, ExternalServiceHealthChecker>();
        return services;
    }
}
