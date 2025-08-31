using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Constants;
using EventAggregator.Shared.MessageBrokers.Enums;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventAggregator.Shared.Infrastructure.Kafka.Producer;

public static class KafkaProducerExtensions
{

    public static IServiceCollection AddKafkaProducerServices<TMessage, TProducer>(this IServiceCollection services,
        IConfigurationSection messageBrokerConfigurationSection,
        TopicType topicType)
            where TMessage : IMessage 
            where TProducer : IProducer<TMessage>
    {
        services.Configure<MessageBrokerConfiguration>(messageBrokerConfigurationSection);

        var brokerConfig = messageBrokerConfigurationSection.Get<MessageBrokerConfiguration>()
            ?? throw new InvalidOperationException("MessageBroker configuration is missing.");

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
