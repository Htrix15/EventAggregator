
namespace EventAggregator.Shared.MessageBrokers.Abstractions;

public abstract record MessageBase: IMessage
{
    public required Guid RequestId { get; set; }
    public required DateTime RequestDateTime { get; set; }
}
