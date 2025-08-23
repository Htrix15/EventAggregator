namespace EventAggregator.Shared.Commands.Abstractions;

public interface ICommandResult
{
    public Guid RequestId { get; }
    public bool IsSuccess { get; }
}
