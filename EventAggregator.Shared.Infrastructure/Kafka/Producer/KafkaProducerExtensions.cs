using EventAggregator.Shared.Infrastructure.Kafka.OverlayConfigurations;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Constants;
using EventAggregator.Shared.MessageBrokers.Enums;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventAggregator.Shared.Infrastructure.Kafka.Producer;

public static class KafkaProducerExtensions
{

    public static IServiceCollection AddKafkaProducerServices<TMessage, TProducer>(this IServiceCollection services,
        IConfigurationSection messageBrokerConfigurationSection,
        TopicType topicType,
        bool overlayMessageBrokerConfiguration)
            where TMessage : IMessage 
            where TProducer : IProducer<TMessage>
    {

        var serviceProvider = services.BuildServiceProvider();

        services.Configure<MessageBrokerConfiguration>(messageBrokerConfigurationSection);

        var brokerConfig = serviceProvider.GetRequiredService<IOptions<MessageBrokerConfiguration>>().Value;

        if (overlayMessageBrokerConfiguration)
        {
            var defaultConfigs = serviceProvider.GetRequiredService<MessageBrokersDefaultConfigurations>();
            var defaultMessageBrokerConfigs = defaultConfigs.GetMessageBrokerConfiguration();
            brokerConfig = KafkaOverlayBrokerConfiguration.Overlay(brokerConfig, defaultMessageBrokerConfigs);
        }

        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers([brokerConfig.BootstrapServers])
                        .CreateTopicIfNotExists(Topics.GetTopic(topicType))
                        .AddProducer<TProducer>(
                            producer => producer
                                .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                        )
                )
        );

        services.AddSingleton<IProducer<TMessage>, KafkaProducer<TMessage>>();

        return services;
    }
}
