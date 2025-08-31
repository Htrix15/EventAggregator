using Confluent.Kafka;
using EventAggregator.Shared.Infrastructure.Kafka.Mapping;
using EventAggregator.Shared.Infrastructure.Kafka.OverlayConfigurations;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Constants;
using EventAggregator.Shared.MessageBrokers.Enums;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace EventAggregator.Shared.Infrastructure.Kafka.Consumer;

public static class KafkaConsumerExtensions
{
    public static IServiceCollection AddKafkaMessageToChannelConsumerServices<TMessage, TMessageHandler>(this IServiceCollection services, 
        IConfigurationSection messageBrokerConfigurationSection,
        IConfigurationSection messageBrokerConsumerConfigurationSection,
        GroupType groupType,
        TopicType topicType,
        bool overlayMessageBrokerConfiguration,
        bool overlayConsumerConfiguration) where TMessageHandler : class, IMessageHandler<TMessage>
    {

        var serviceProvider = services.BuildServiceProvider();
        var defaultConfigs = serviceProvider
            .GetRequiredService<MessageBrokersDefaultConfigurations>();
          
        services.Configure<MessageBrokerConfiguration>(messageBrokerConfigurationSection);
        services.Configure<MessageBrokerMessageToChannelConsumerConfiguration>(messageBrokerConsumerConfigurationSection);

        var brokerConfig = serviceProvider.GetRequiredService<IOptions<MessageBrokerConfiguration>>().Value;
        var consumerConfig = serviceProvider.GetRequiredService<IOptions<MessageBrokerMessageToChannelConsumerConfiguration>>().Value;

        if (overlayMessageBrokerConfiguration)
        {
            var defaultMessageBrokerConfigs = defaultConfigs.GetMessageBrokerConfiguration();
            brokerConfig = KafkaOverlayBrokerConfiguration.Overlay(brokerConfig, defaultMessageBrokerConfigs);
        }

        if (overlayConsumerConfiguration)
        {
            var defaultMessageToChannelConsumerConfig = defaultConfigs.GetMessageBrokerMessageToChannelConsumerConfiguration();
            consumerConfig = KafkaOverlayConsumerMessageToChannelSettings.Overlay(consumerConfig, defaultMessageToChannelConsumerConfig);
        }

        services.AddSingleton(provider =>
            consumerConfig.UnboundedChannel!.Value
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
                            AutoOffsetReset = AutoOffsetResetMapping.Map(consumerConfig.AutoOffsetReset!.Value),
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
