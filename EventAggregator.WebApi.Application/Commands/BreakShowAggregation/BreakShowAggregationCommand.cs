using EventAggregator.Shared.Commands.Abstractions;

namespace EventAggregator.WebApi.Application.Commands.BreakShowAggregation;

public record BreakShowAggregationCommand : ICommand
{
    public required Guid RequestId { get; init; }
}
