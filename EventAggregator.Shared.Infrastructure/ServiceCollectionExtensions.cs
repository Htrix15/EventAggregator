using EventAggregator.Shared.ExternalServices.Services;
using EventAggregator.Shared.Infrastructure.Kafka;
using EventAggregator.Shared.Infrastructure.Services;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventAggregator.Shared.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedExternalServiceInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IExternalServiceHealthChecker, ExternalServiceHealthChecker>();
        return services;
    }

    public static IServiceCollection AddSharedKafkaProducerInfrastructure<TMessage>(this IServiceCollection services, 
        IConfigurationSection configurationSection) where TMessage: IMessage
    {
        services.Configure<MessageBrokerConfiguration>(configurationSection);
        services.AddSingleton<MessageBrokersDefaultConfigurations>();
        services.AddSingleton<IProducer<TMessage>, KafkaProducer<TMessage>>();
        return services;
    }
}
