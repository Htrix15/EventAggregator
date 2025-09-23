
namespace EventAggregator.Shared.MessageBrokers.Abstractions;

public abstract record MessageBase(Guid RequestId, DateTime RequestDateTime) : IMessage;