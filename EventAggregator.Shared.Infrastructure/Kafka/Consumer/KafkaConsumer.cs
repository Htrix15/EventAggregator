using Confluent.Kafka;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Constants;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;

namespace EventAggregator.Shared.Infrastructure.Kafka.Consumer;

public class KafkaConsumer<TMessage> : BackgroundService, IDisposable
    where TMessage : IMessage
{
    private readonly MessageBrokerConsumerToChannelConfiguration _messageBrokerConsumerConfiguration;
    private readonly IConsumer<Ignore, TMessage> _consumer;
    private readonly Channel<TMessage> _messageHanlerChannel;

    public KafkaConsumer(MessageBrokerConfiguration messageBrokerConfiguration,
        MessageBrokerConsumerToChannelConfiguration messageBrokerConsumerConfiguration,
        Channel<TMessage> messageHanlerChannel)
    {
        var consumerConfig = new ConsumerConfig()
        {
            ClientId = messageBrokerConfiguration.Client.ToString()
               ?? throw new InvalidOperationException("MessageBrokerConfiguration - ClientId not definition"),

            GroupId = messageBrokerConsumerConfiguration.Group.ToString()
               ?? throw new InvalidOperationException("MessageBrokerConfiguration - GroupId not definition"),

            BootstrapServers = messageBrokerConfiguration.BootstrapServers
               ?? throw new InvalidOperationException("MessageBrokerConfiguration - BootstrapServers not definition"),

            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<Ignore, TMessage>(consumerConfig)
            .SetValueDeserializer(new KafkaJsonDeserializer<TMessage>())
            .Build();

        _messageBrokerConsumerConfiguration = messageBrokerConsumerConfiguration;
        _messageHanlerChannel = messageHanlerChannel;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        _consumer.Subscribe(Topics.GetTopic(_messageBrokerConsumerConfiguration.Topic));

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(cancellationToken);
                if (result == null) continue;

                await _messageHanlerChannel.Writer.WriteAsync(result.Message.Value, cancellationToken);
                _consumer.Commit(result);
            }
        }
        finally
        {
            _consumer.Close();
        }
    }

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
    }
}
