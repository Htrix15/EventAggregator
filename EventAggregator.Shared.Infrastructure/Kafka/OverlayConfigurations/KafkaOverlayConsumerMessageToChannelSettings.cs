using Confluent.Kafka;
using EventAggregator.Shared.MessageBrokers.Configuration;

namespace EventAggregator.Shared.Infrastructure.Kafka.OverlayConfigurations;

public static class KafkaOverlayConsumerMessageToChannelSettings
{
    public static MessageBrokerMessageToChannelConsumerConfiguration Overlay(
        MessageBrokerMessageToChannelConsumerConfiguration config,
        MessageBrokerMessageToChannelConsumerConfiguration defaultConfig)
    {
        return new MessageBrokerMessageToChannelConsumerConfiguration()
        {
            BufferSize = config?.BufferSize ?? defaultConfig.BufferSize,
            WorkersCount = config?.WorkersCount ?? defaultConfig.WorkersCount,
            SessionTimeoutMs = config?.SessionTimeoutMs ?? defaultConfig.SessionTimeoutMs,
            MaxPollIntervalMs = config?.MaxPollIntervalMs ?? defaultConfig.MaxPollIntervalMs,
            HeartbeatIntervalMs = config?.HeartbeatIntervalMs ?? defaultConfig.HeartbeatIntervalMs,
            EnableAutoCommit = config?.EnableAutoCommit ?? defaultConfig.EnableAutoCommit,
            EnableAutoOffsetStore = config?.EnableAutoOffsetStore ?? defaultConfig.EnableAutoOffsetStore,
            AutoOffsetReset = config?.AutoOffsetReset ?? defaultConfig.AutoOffsetReset,
            UnboundedChannel = config?.UnboundedChannel ?? defaultConfig.UnboundedChannel,
            ChannelFullMode = config?.ChannelFullMode ?? defaultConfig.ChannelFullMode,
            ChannelCapacity = config?.ChannelCapacity ?? defaultConfig.ChannelCapacity,
        };
    }
}
