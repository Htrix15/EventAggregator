namespace EventAggregator.Shared.MessageBrokers.Abstractions;

public interface IMessage
{
    public Guid RequestId { get; init; }
}
