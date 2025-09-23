using EventAggregator.Kassir.Application.Commands.NotifySearchShowStarted;
using EventAggregator.Kassir.Application.Commands.ReturnShowList;
using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.Shared.ShowEntities.Models;

namespace EventAggregator.Kassir.Application.Commands.StartShowAggregation;

public class StartShowAggregationHandler(ICommandHandler<NotifySearchShowStartedCommand> notifySearchShowStartedCommandHandler,
    ICommandHandler<ReturnShowListCommand> returnShowListCommandHandler
    ) : ICommandHandler<StartShowAggregationCommand>
{
    public async Task<ICommandResult> Handle(StartShowAggregationCommand command, CancellationToken cancellationToken = default)
    {
        
        await notifySearchShowStartedCommandHandler.Handle(new NotifySearchShowStartedCommand(command.RequestId, command.SearchDateRanges, command.ShowTypes), cancellationToken);
        
        var shows = new List<Show>(); //TODO Create
        
        return await returnShowListCommandHandler.Handle(new ReturnShowListCommand(command.RequestId, shows), cancellationToken);

    }
}
