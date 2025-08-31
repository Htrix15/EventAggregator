using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.Shared.ShowEntities.Enums;
using EventAggregator.Shared.ShowEntities.ValueObjects;

namespace EventAggregator.WebApi.Application.Commands.StartShowAggregation;

public record StartShowAggregationCommand : ICommand
{
    public required List<DateRange> SearchDateRanges { get; init; }
    public required List<ShowType> ShowTypes { get; init; }
}
