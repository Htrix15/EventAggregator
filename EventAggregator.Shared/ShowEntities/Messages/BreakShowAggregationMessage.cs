using EventAggregator.Shared.MessageBrokers.Abstractions;

namespace EventAggregator.Shared.ShowEntities.Messages;

public record BreakShowAggregationMessage(Guid RequestId, DateTime RequestDateTime) : MessageBase(RequestId, RequestDateTime);
