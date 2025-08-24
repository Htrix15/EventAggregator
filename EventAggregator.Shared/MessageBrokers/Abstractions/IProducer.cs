using EventAggregator.Shared.MessageBrokers.Enums;

namespace EventAggregator.Shared.MessageBrokers.Abstractions;

public interface IProducer<TMessage> : IDisposable where TMessage : IMessage
{
    Task SendMessage(Guid requestId, TMessage message, TopicType topic, CancellationToken cancellationToken);
}
