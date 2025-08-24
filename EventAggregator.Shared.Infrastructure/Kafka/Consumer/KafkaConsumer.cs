using Confluent.Kafka;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EventAggregator.Shared.Infrastructure.Kafka.Consumer;

public class KafkaConsumer<TMessage> : BackgroundService where TMessage: IMessage
{
    private readonly IConsumer<string, TMessage> _consumer;
    private readonly TopicType[] _topics;

    public KafkaConsumer(IOptions<MessageBrokerConfiguration> messageBrokerConfiguration,
        IOptions<MessageBrokerConsumerConfiguration> messageBrokerConsumerConfiguration,
        MessageBrokersDefaultConfigurations defaultConfigurationDictionary)
    {
        var options = messageBrokerConfiguration.Value;
        var defaultOptions = defaultConfigurationDictionary.GetMessageBrokerConfiguration();

        _topics = messageBrokerConsumerConfiguration.Value?.ListeningTopics
            ?? throw new InvalidOperationException("MessageBrokerConfiguration - ListeningTopics not definition");

        var consumerConfig = new ConsumerConfig()
        {
            ClientId = options.Client?.ToString()
               ?? defaultOptions.Client?.ToString()
               ?? throw new InvalidOperationException("MessageBrokerConfiguration - ClientId not definition"),

            GroupId = options.Group?.ToString()
               ?? defaultOptions.Group?.ToString()
               ?? throw new InvalidOperationException("MessageBrokerConfiguration - GroupId not definition"),

            BootstrapServers = options.BootstrapServers?.ToString()
               ?? defaultOptions.BootstrapServers?.ToString()
               ?? throw new InvalidOperationException("MessageBrokerConfiguration - BootstrapServers not definition"),
        };

        _consumer = new ConsumerBuilder<string, TMessage>(consumerConfig)
            .SetValueDeserializer(new KafkaJsonDeserializer<TMessage>())
            .Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
