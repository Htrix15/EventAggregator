using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.Shared.Commands.Implementations;
using EventAggregator.Shared.ExternalServices.Enums;
using EventAggregator.Shared.ExternalServices.Services;

namespace EventAggregator.WebApi.Application.Commands.StartShowAggregation;

public class StartShowAggregationCommandHandler(IExternalServiceHealthChecker externalServiceHealthChecker) : ICommandHandler<StartShowAggregationCommand>
{
    public async Task<ICommandResult> Handle(StartShowAggregationCommand command, CancellationToken cancellationToken = default)
    {
        //TODO Use cache, refresh in background on timeout
        var orchestratorHealthy = await externalServiceHealthChecker.IsServiceHealthy(ExternalServiceType.Orchestrator.ToString(), 
            cancellationToken);
        if (!orchestratorHealthy)
        {
            return new CommandResult()
            {
                RequestId = new Guid(),
                IsSuccess = false
            };
        }
        throw new NotImplementedException();
    }
}
