using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.ShowEntities.Enums;
using EventAggregator.Shared.ShowEntities.ValueObjects;

namespace EventAggregator.Shared.ShowEntities.Messages;

public record StartShowAggregationMessage(Guid RequestId, DateTime RequestDateTime, List<DateRange> SearchDateRanges, List<ShowType> ShowTypes) 
    : MessageBase(RequestId, RequestDateTime);