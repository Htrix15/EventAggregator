using Confluent.Kafka;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Constants;
using EventAggregator.Shared.MessageBrokers.Enums;
using Microsoft.Extensions.Options;

namespace EventAggregator.Shared.Infrastructure.Kafka;

public class KafkaProducer<TMessage> : IProducer<TMessage> where TMessage: IMessage
{
    private readonly IProducer<string, TMessage> _producer;
    private readonly int? _flushTimeoutMs;

    public KafkaProducer(IOptions<MessageBrokerConfiguration> kafkaServiceDefinition,
        MessageBrokersDefaultConfigurations defaultConfigurationDictionary)
    {
        var options = kafkaServiceDefinition.Value;
        var defaultOptions = defaultConfigurationDictionary.GetMessageBrokerConfiguration();

        var producerConfig = new ProducerConfig()
        {
            ClientId = options.ClientId?.ToString()
                ?? defaultOptions.ClientId?.ToString()
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - ClientId not definition"),

            BootstrapServers = options.BootstrapServers?.ToString()
                ?? defaultOptions.BootstrapServers?.ToString()
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - BootstrapServers not definition"),

            EnableIdempotence = options.EnableIdempotence
                ?? defaultOptions.EnableIdempotence
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - EnableIdempotence not definition"),

            MessageSendMaxRetries = options.MessageSendMaxRetries
                ?? defaultOptions.MessageSendMaxRetries
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - MessageSendMaxRetries not definition"),

            RetryBackoffMs = options.RetryBackoffMs
                ?? defaultOptions.RetryBackoffMs
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - RetryBackoffMs not definition"),

            Acks = (options.CheckSending 
                ?? defaultOptions.CheckSending 
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - CheckSending not definition"))
                    ? (options.EnableIdempotence 
                            ?? defaultOptions.EnableIdempotence 
                            ?? throw new InvalidOperationException("MessageBrokerConfiguration - EnableIdempotence not definition"))
                        ? Acks.All
                        : Acks.Leader
                : Acks.None
        };

        _flushTimeoutMs = options.FlushTimeoutMs;

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
            await _producer.ProduceAsync(TopicsDictionary.GetTopic(topic), new Message<string, TMessage>()
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
