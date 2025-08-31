using EventAggregator.Shared.MessageBrokers.Abstractions;
using KafkaFlow;

using System.Threading.Channels;

namespace EventAggregator.Shared.Infrastructure.Kafka.Consumer;

public class KafkaConsumerMessageToChannelHandler<TMessage>(Channel<TMessage> channel) : IMessageHandler<TMessage> where TMessage : IMessage
{
    public async Task Handle(IMessageContext context, TMessage message)
    {
        try
        {
            await channel.Writer.WriteAsync(message);
        }
        catch
        {
            // TODO log error  
        }
    }
}
