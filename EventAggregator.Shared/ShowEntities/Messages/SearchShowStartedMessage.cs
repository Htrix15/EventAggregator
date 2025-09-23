using EventAggregator.Shared.MessageBrokers.Abstractions;

namespace EventAggregator.Shared.ShowEntities.Messages;

public record SearchShowStartedMessage(Guid RequestId, 
    DateTime RequestDateTime, 
    bool Started, 
    int EstimatedCompletionSeconds) : MessageBase(RequestId, RequestDateTime);
