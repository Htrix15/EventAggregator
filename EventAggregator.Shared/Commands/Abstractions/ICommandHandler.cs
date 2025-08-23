namespace EventAggregator.Shared.Commands.Abstractions;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task<ICommandResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}