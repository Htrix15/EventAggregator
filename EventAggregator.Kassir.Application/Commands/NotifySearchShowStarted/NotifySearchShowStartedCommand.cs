using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.Shared.ShowEntities.Enums;
using EventAggregator.Shared.ShowEntities.ValueObjects;

namespace EventAggregator.Kassir.Application.Commands.NotifySearchShowStarted;

public record NotifySearchShowStartedCommand(Guid RequestId, List<DateRange> SearchDateRanges, List<ShowType> ShowTypes) : ICommand;
