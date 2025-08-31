using EventAggregator.Shared.ExternalServices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventAggregator.Shared.Infrastructure.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedExternalServiceInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IExternalServiceHealthChecker, ExternalServiceHealthChecker>();
        return services;
    }
}
