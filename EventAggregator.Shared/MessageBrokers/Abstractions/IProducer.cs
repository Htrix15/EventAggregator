using EventAggregator.Shared.MessageBrokers.Enums;

namespace EventAggregator.Shared.MessageBrokers.Abstractions;

public interface IProducer<TMessage> where TMessage : IMessage
{
    Task SendMessage(string messageKey, TMessage message, TopicType topic, CancellationToken cancellationToken);
}
