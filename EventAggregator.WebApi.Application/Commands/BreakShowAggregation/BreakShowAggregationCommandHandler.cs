using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.Shared.Commands.Implementations;
using EventAggregator.Shared.ExternalServices.Enums;
using EventAggregator.Shared.ExternalServices.Services;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Enums;
using EventAggregator.Shared.ShowEntities.Messages;

namespace EventAggregator.WebApi.Application.Commands.BreakShowAggregation;


public class BreakShowAggregationCommandHandler(IExternalServiceHealthChecker externalServiceHealthChecker,
    IProducer<BreakShowAggregationMessage> producer) : ICommandHandler<BreakShowAggregationCommand>
{
    public async Task<ICommandResult> Handle(BreakShowAggregationCommand command, CancellationToken cancellationToken = default)
    {
        //TODO Use cache, refresh in background on timeout
        var orchestratorHealthy = await externalServiceHealthChecker.IsServiceHealthy(ExternalServiceType.Orchestrator.ToString(),
            cancellationToken);

        if (!orchestratorHealthy)
        {
            return new CommandResult()
            {
                RequestId = command.RequestId,
                IsSuccess = false
            };
        }

        var message = new BreakShowAggregationMessage(command.RequestId, DateTime.UtcNow);

        try
        {
            await producer.SendMessage(messageKey: command.RequestId.ToString(), message, TopicType.BreakShowAggregation, cancellationToken);
        }
        catch
        {
            return new CommandResult()
            {
                RequestId = command.RequestId,
                IsSuccess = false
            };
        }

        return new CommandResult()
        {
            RequestId = command.RequestId,
            IsSuccess = true
        };
    }
}