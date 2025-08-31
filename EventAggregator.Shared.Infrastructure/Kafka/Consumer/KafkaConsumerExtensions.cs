using Confluent.Kafka;
using EventAggregator.Shared.Infrastructure.Kafka.Mapping;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Constants;
using EventAggregator.Shared.MessageBrokers.Enums;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace EventAggregator.Shared.Infrastructure.Kafka.Consumer;

public static class KafkaConsumerExtensions
{
    public static IServiceCollection AddKafkaMessageToChannelConsumerServices<TMessage, TMessageHandler>(this IServiceCollection services, 
        IConfigurationSection messageBrokerConfigurationSection,
        IConfigurationSection messageBrokerConsumerConfigurationSection,
        GroupType groupType,
        TopicType topicType) where TMessageHandler : class, IMessageHandler<TMessage>
    {

        services.Configure<MessageBrokerConfiguration>(messageBrokerConfigurationSection);
        services.Configure<MessageBrokerMessageToChannelConsumerConfiguration>(messageBrokerConsumerConfigurationSection);

        var brokerConfig = messageBrokerConfigurationSection.Get<MessageBrokerConfiguration>()
          ?? throw new InvalidOperationException("MessageBroker configuration is missing.");

        var consumerConfig = messageBrokerConsumerConfigurationSection.Get<MessageBrokerMessageToChannelConsumerConfiguration>()
          ?? throw new InvalidOperationException("MessageBrokerMessageToChannelConsumer configuration is missing.");

        services.AddSingleton(provider =>
            consumerConfig.UnboundedChannel
                ? Channel.CreateUnbounded<TMessage>()
                : Channel.CreateBounded<TMessage>(new BoundedChannelOptions(consumerConfig.ChannelCapacity!.Value)
                    {
                        FullMode = consumerConfig.ChannelFullMode
                    }));

        services.AddKafka(kafka =>
        {
            kafka
                .UseConsoleLog()
                .AddCluster(cluster => cluster
                    .WithBrokers([brokerConfig.BootstrapServers])
                    .AddConsumer(consumer => consumer
                        .Topic(Topics.GetTopic(topicType))
                        .WithGroupId(Groups.GetGroup(groupType))
                        .WithBufferSize(consumerConfig.BufferSize!.Value)
                        .WithWorkersCount(consumerConfig.WorkersCount!.Value)
                        .WithConsumerConfig(new ConsumerConfig
                        {
                            SessionTimeoutMs = consumerConfig.SessionTimeoutMs,
                            MaxPollIntervalMs = consumerConfig.MaxPollIntervalMs,
                            HeartbeatIntervalMs = consumerConfig.HeartbeatIntervalMs,
                            AutoOffsetReset = AutoOffsetResetMapping.Map(consumerConfig.AutoOffsetReset),
                            EnableAutoCommit = consumerConfig.EnableAutoCommit,
                            EnableAutoOffsetStore = consumerConfig.EnableAutoOffsetStore
                        })
                        .AddMiddlewares(m => m.AddDeserializer<JsonCoreDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<TMessageHandler>()))));
        });

        return services;
    }

    public static async Task StartKafkaBus(this WebApplication app)
    {
        var kafkaBus = app.Services.CreateKafkaBus();
        await kafkaBus.StartAsync();
    }
}
