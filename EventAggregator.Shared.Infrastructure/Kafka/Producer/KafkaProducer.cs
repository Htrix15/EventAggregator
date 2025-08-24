using Confluent.Kafka;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Constants;
using EventAggregator.Shared.MessageBrokers.Enums;
using Microsoft.Extensions.Options;

namespace EventAggregator.Shared.Infrastructure.Kafka.Producer;

public class KafkaProducer<TMessage> : IProducer<TMessage> where TMessage: IMessage
{
    private readonly IProducer<string, TMessage> _producer;
    private readonly int? _flushTimeoutMs;

    public KafkaProducer(IOptions<MessageBrokerConfiguration> messageBrokerConfiguration,
        IOptions<MessageBrokerProducerConfiguration> messageBrokerProducerConfiguration,
        MessageBrokersDefaultConfigurations defaultConfigurationDictionary)
    {
        var messageBrokerOptions = messageBrokerConfiguration.Value;
        var defaultMessageBrokerOptions = defaultConfigurationDictionary.GetMessageBrokerConfiguration();

        var messageBrokerProducerOptions = messageBrokerProducerConfiguration.Value;
        var defaultMessageBrokerProducerOptions = defaultConfigurationDictionary.GetMessageBrokerProducerConfiguration();

        var producerConfig = new ProducerConfig()
        {
            ClientId = messageBrokerOptions.Client?.ToString()
                ?? defaultMessageBrokerOptions.Client?.ToString()
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - ClientId not definition"),

            BootstrapServers = messageBrokerOptions.BootstrapServers?.ToString()
                ?? defaultMessageBrokerOptions.BootstrapServers?.ToString()
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - BootstrapServers not definition"),

            EnableIdempotence = messageBrokerProducerOptions.EnableIdempotence
                ?? defaultMessageBrokerProducerOptions.EnableIdempotence
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - EnableIdempotence not definition"),

            MessageSendMaxRetries = messageBrokerProducerOptions.MessageSendMaxRetries
                ?? defaultMessageBrokerProducerOptions.MessageSendMaxRetries
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - MessageSendMaxRetries not definition"),

            RetryBackoffMs = messageBrokerProducerOptions.RetryBackoffMs
                ?? defaultMessageBrokerProducerOptions.RetryBackoffMs
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - RetryBackoffMs not definition"),

            Acks = messageBrokerProducerOptions.CheckSending 
                ?? defaultMessageBrokerProducerOptions.CheckSending 
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - CheckSending not definition")
                    ? messageBrokerProducerOptions.EnableIdempotence 
                            ?? defaultMessageBrokerProducerOptions.EnableIdempotence 
                            ?? throw new InvalidOperationException("MessageBrokerConfiguration - EnableIdempotence not definition")
                        ? Acks.All
                        : Acks.Leader
                : Acks.None
        };

        _flushTimeoutMs = messageBrokerProducerOptions.FlushTimeoutMs ?? defaultMessageBrokerProducerOptions.FlushTimeoutMs;

        _producer = new ProducerBuilder<string, TMessage>(producerConfig)
            .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
            .Build();
    }

    public async Task SendMessage(Guid requestId,
        TMessage message, 
        TopicType topic, 
        CancellationToken cancellationToken)
    {
        try
        {
            await _producer.ProduceAsync(Topics.GetTopic(topic), new Message<string, TMessage>()
            {
                Key = requestId.ToString(),
                Value = message
            },
            cancellationToken);
        }    
        catch (ProduceException<string, string> ex)
        {
            //TODO Log errors
        }
        catch (Exception ex)
        {
            //TODO Log errors
        }
    }

    public void Dispose()
    {
        try
        {
            if (_flushTimeoutMs.HasValue)
            {
                _producer.Flush(TimeSpan.FromMilliseconds(_flushTimeoutMs.Value));
            }
        }
        finally
        {
            _producer?.Dispose();
        }
    }
}
