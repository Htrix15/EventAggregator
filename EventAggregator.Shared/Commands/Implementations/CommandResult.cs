using EventAggregator.Shared.Commands.Abstractions;

namespace EventAggregator.Shared.Commands.Implementations;

public readonly record struct CommandResult : ICommandResult
{
    public Guid RequestId { get; init; }
    public bool IsSuccess { get; init; }
}
