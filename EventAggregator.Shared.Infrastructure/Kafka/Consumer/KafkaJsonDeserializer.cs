using Confluent.Kafka;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using System.Text.Json;

namespace EventAggregator.Shared.Infrastructure.Kafka.Consumer;

internal class KafkaJsonDeserializer<TMessage> : IDeserializer<TMessage> where TMessage : IMessage
{
    public TMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return JsonSerializer.Deserialize<TMessage>(data)!;
    }
}
