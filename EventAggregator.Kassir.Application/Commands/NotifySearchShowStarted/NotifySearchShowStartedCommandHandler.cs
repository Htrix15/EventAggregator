using EventAggregator.Kassir.Domain.Services;
using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.Shared.Commands.Implementations;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Enums;
using EventAggregator.Shared.ShowEntities.Messages;

namespace EventAggregator.Kassir.Application.Commands.NotifySearchShowStarted;

public class NotifySearchShowStartedCommandHandler(IProducer<SearchShowStartedMessage> producer,
    IEstimateCompletionTimeService estimateCompletionTimeService) : ICommandHandler<NotifySearchShowStartedCommand>
{
    public async Task<ICommandResult> Handle(NotifySearchShowStartedCommand command, CancellationToken cancellationToken = default)
    {
        var requestId = command.RequestId;

        var message = new SearchShowStartedMessage(RequestId: requestId, 
            RequestDateTime: DateTime.UtcNow, 
            Started: true,
            EstimatedCompletionSeconds: estimateCompletionTimeService.CalculateEstimatedCompletionTimeSecond(command.SearchDateRanges, command.ShowTypes));

        try
        {
            await producer.SendMessage(messageKey: requestId.ToString(), message, TopicType.SearchShowStarted, cancellationToken);
        }
        catch
        {
            return new CommandResult()
            {
                RequestId = requestId,
                IsSuccess = false
            };
        }

        return new CommandResult()
        {
            RequestId = requestId,
            IsSuccess = true
        };
    }
}
