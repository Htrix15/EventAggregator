using EventAggregator.Shared.MessageBrokers.Configuration;

namespace EventAggregator.Shared.Infrastructure.Kafka.OverlayConfigurations;

public static class KafkaOverlayBrokerConfiguration
{
    public static MessageBrokerConfiguration Overlay(MessageBrokerConfiguration config,
        MessageBrokerConfiguration defaultConfig)
    {
        return new MessageBrokerConfiguration()
        {
            BootstrapServers = config?.BootstrapServers ?? defaultConfig.BootstrapServers,
            Client = config?.Client ?? defaultConfig.Client,
            Group = config?.Group ?? defaultConfig.Group,
        };
    }
}
