using EventAggregator.Shared.ShowEntities.Messages;

namespace EventAggregator.Kassir.Domain.Services;

public interface IMessageManagerService 
{
    Func<StartShowAggregationMessage, CancellationToken, Task> GetActionHandler();
}