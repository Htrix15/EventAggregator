using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.ShowEntities.Enums;
using EventAggregator.Shared.ShowEntities.ValueObjects;

namespace EventAggregator.Shared.ShowEntities.Messages;

public record StartShowAggregationMessage : MessageBase
{
    public required List<DateRange> SearchDateRanges { get; set; }
    public required List<ShowType> ShowTypes { get; set; }
}
