using Confluent.Kafka;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Constants;
using EventAggregator.Shared.MessageBrokers.Enums;

namespace EventAggregator.Shared.Infrastructure.Kafka.Producer;

public class KafkaProducer<TMessage> : IDisposable, IProducer<TMessage> where TMessage: IMessage
{
    private readonly IProducer<string, TMessage> _producer;
    private readonly int? _flushTimeoutMs;

    public KafkaProducer(MessageBrokerConfiguration messageBrokerConfiguration,
        MessageBrokerProducerConfiguration messageBrokerProducerConfiguration)
    {
        var producerConfig = new ProducerConfig()
        {
            ClientId = messageBrokerConfiguration.Client.ToString(),

            BootstrapServers = messageBrokerConfiguration.BootstrapServers,

            EnableIdempotence = messageBrokerProducerConfiguration.EnableIdempotence
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - EnableIdempotence not definition"),

            MessageSendMaxRetries = messageBrokerProducerConfiguration.MessageSendMaxRetries
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - MessageSendMaxRetries not definition"),

            RetryBackoffMs = messageBrokerProducerConfiguration.RetryBackoffMs
   
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - RetryBackoffMs not definition"),

            Acks = messageBrokerProducerConfiguration.CheckSending 
                ?? throw new InvalidOperationException("MessageBrokerConfiguration - CheckSending not definition")
                    ? messageBrokerProducerConfiguration.EnableIdempotence 
                            ?? throw new InvalidOperationException("MessageBrokerConfiguration - EnableIdempotence not definition")
                        ? Acks.All
                        : Acks.Leader
                : Acks.None
        };

        _flushTimeoutMs = messageBrokerProducerConfiguration.FlushTimeoutMs;

        _producer = new ProducerBuilder<string, TMessage>(producerConfig)
            .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
            .Build();
    }

    public async Task SendMessage(string requestId,
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
