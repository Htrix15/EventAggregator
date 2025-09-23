using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.Shared.ShowEntities.Enums;
using EventAggregator.Shared.ShowEntities.ValueObjects;

namespace EventAggregator.Kassir.Application.Commands.StartShowAggregation;

public record StartShowAggregationCommand(Guid RequestId, List<DateRange> SearchDateRanges, List<ShowType> ShowTypes) : ICommand;
