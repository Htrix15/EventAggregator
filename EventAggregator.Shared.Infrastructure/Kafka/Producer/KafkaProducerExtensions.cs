using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventAggregator.Shared.Infrastructure.Kafka.Producer;

public static class KafkaProducerExtensions
{
    public static IServiceCollection AddKafkaProducer<TMessage>(this IServiceCollection services,
       IConfigurationSection messageBrokerConfigurationSection,
       IConfigurationSection messageBrokerProducerConfigurationSection,
       string optionsKey) where TMessage : IMessage
    {
        services.Configure<MessageBrokerConfiguration>(optionsKey, messageBrokerConfigurationSection);
        services.Configure<MessageBrokerProducerConfiguration>(optionsKey, messageBrokerProducerConfigurationSection.GetSection(optionsKey));

        services.AddSingleton<IProducer<TMessage>>(sp =>
        {
            var messageBrokerConfigurationMonitor = sp.GetRequiredService<IOptionsMonitor<MessageBrokerConfiguration>>();
            var producerConfigurationMonitor = sp.GetRequiredService<IOptionsMonitor<MessageBrokerProducerConfiguration>>();

            var messageBrokerConfiguration = messageBrokerConfigurationMonitor.Get(optionsKey)!; 
            var producerConfiguration = producerConfigurationMonitor.Get(optionsKey)!; 

            return new KafkaProducer<TMessage>(messageBrokerConfiguration, producerConfiguration);
        });

        return services;
    }
}
