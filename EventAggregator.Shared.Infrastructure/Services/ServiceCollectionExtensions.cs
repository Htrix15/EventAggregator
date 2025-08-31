using EventAggregator.Shared.ExternalServices.Services;
using EventAggregator.Shared.Infrastructure.Kafka.Producer;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using Microsoft.Extensions.Configuration;
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
