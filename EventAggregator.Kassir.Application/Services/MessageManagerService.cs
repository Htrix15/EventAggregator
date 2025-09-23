using EventAggregator.Kassir.Application.Commands.StartShowAggregation;
using EventAggregator.Kassir.Domain.Services;
using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.Shared.ShowEntities.Messages;

namespace EventAggregator.Kassir.Application.Services;

public class MessageManagerService(ICommandHandler<StartShowAggregationCommand> startShowAggregationCommandHandler) : IMessageManagerService
{
    public Func<StartShowAggregationMessage, CancellationToken, Task> GetActionHandler() 
    {
        return (m, c) =>
        {
            var command = new StartShowAggregationCommand(m.RequestId, m.SearchDateRanges, m.ShowTypes);
            return startShowAggregationCommandHandler.Handle(command, c);
        };
    }
}
