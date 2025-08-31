using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace EventAggregator.Shared.Infrastructure.Kafka.Consumer;

public static class KafkaConsumerExtensions
{

    public static IServiceCollection AddKafkaConsumer<TMessage>(this IServiceCollection services,
        IConfigurationSection messageBrokerConfigurationSection,
        IConfigurationSection messageBrokerConsumerToChannelConfigurationSection,
        string optionsKey) where TMessage : IMessage
    {
        services.Configure<MessageBrokerConfiguration>(optionsKey, messageBrokerConfigurationSection);
        services.Configure<MessageBrokerConsumerToChannelConfiguration>(optionsKey, messageBrokerConsumerToChannelConfigurationSection.GetSection(optionsKey));

        services.AddSingleton(sp =>
        {
            var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<MessageBrokerConsumerToChannelConfiguration>>();
            var channelConfig = optionsMonitor.Get(optionsKey).ChannelConfiguration;

            if (channelConfig.Unbounded)
            {
                return Channel.CreateUnbounded<TMessage>();
            }

            return Channel.CreateBounded<TMessage>(new BoundedChannelOptions(channelConfig.Capacity)
            {
                FullMode = channelConfig.FullMode
            });
        });

        services.AddHostedService(sp =>
        {
            var messageBrokerConfigurationMonitor = sp.GetRequiredService<IOptionsMonitor<MessageBrokerConfiguration>>();
            var consumerToChannelConfigurationMonitor = sp.GetRequiredService<IOptionsMonitor<MessageBrokerConsumerToChannelConfiguration>>();

            var messageBrokerConfiguration = messageBrokerConfigurationMonitor.Get(optionsKey)!;
            var consumerToChannelConfiguration = consumerToChannelConfigurationMonitor.Get(optionsKey)!;

            var channel = sp.GetRequiredService<Channel<TMessage>>();

            return new KafkaConsumer<TMessage>(messageBrokerConfiguration, consumerToChannelConfiguration, channel);
        }); 

        return services;
    }
}
