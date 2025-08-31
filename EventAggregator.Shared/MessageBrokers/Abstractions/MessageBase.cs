
namespace EventAggregator.Shared.MessageBrokers.Abstractions;

public abstract record MessageBase: IMessage
{
    public required Guid RequestId { get; init; }
    public required DateTime RequestDateTime { get; init; }
}
