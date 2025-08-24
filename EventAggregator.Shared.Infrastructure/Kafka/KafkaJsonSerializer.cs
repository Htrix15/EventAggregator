using Confluent.Kafka;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using System.Text.Json;

namespace EventAggregator.Shared.Infrastructure.Kafka;

internal class KafkaJsonSerializer<TMessage> : ISerializer<TMessage> where TMessage : IMessage
{
    public byte[] Serialize(TMessage data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}
