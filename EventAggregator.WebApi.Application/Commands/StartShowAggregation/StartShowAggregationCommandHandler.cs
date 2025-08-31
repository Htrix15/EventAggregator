using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.Shared.Commands.Implementations;
using EventAggregator.Shared.ExternalServices.Enums;
using EventAggregator.Shared.ExternalServices.Services;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Enums;
using EventAggregator.Shared.ShowEntities.Messages;

namespace EventAggregator.WebApi.Application.Commands.StartShowAggregation;

public class StartShowAggregationCommandHandler(IExternalServiceHealthChecker externalServiceHealthChecker,
    IProducer<StartShowAggregationMessage> producer) : ICommandHandler<StartShowAggregationCommand>
{
    public async Task<ICommandResult> Handle(StartShowAggregationCommand command, CancellationToken cancellationToken = default)
    {
        var requestId = Guid.NewGuid();

        //TODO Use cache, refresh in background on timeout
        var orchestratorHealthy = await externalServiceHealthChecker.IsServiceHealthy(ExternalServiceType.Orchestrator.ToString(),
            cancellationToken);

        if (!orchestratorHealthy)
        {
            return new CommandResult()
            {
                RequestId = requestId,
                IsSuccess = false
            };
        }

        var message = new StartShowAggregationMessage()
        {
            RequestId = requestId,
            RequestDateTime = DateTime.UtcNow,
            SearchDateRanges = command.SearchDateRanges,
            ShowTypes = command.ShowTypes
        };

        try
        {
            await producer.SendMessage(messageKey: requestId.ToString(), message, TopicType.StartShowAggregation, cancellationToken);
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
