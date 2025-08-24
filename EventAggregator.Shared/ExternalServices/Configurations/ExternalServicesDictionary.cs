using EventAggregator.Shared.ExternalServices.Enums;
using EventAggregator.Shared.ExternalServices.ValueObjects;
using Microsoft.Extensions.Hosting;

namespace EventAggregator.Shared.ExternalServices.Configurations;

public static class ExternalServicesDictionary
{
    private readonly static Dictionary<string, Dictionary<ExternalServiceType, ExternalServiceDefinition>> _externalServices = new()
    {
        [Environments.Development] = new Dictionary<ExternalServiceType, ExternalServiceDefinition>
            {
                [ExternalServiceType.Orchestrator] = new ExternalServiceDefinition
                {
                    BaseUri = "https://localhost:44330/"
                }
        },
    };

    public static ExternalServiceDefinition GetExternalServiceDefinition(string environment, ExternalServiceType serviceType)
    {
        if (_externalServices.TryGetValue(environment, out var definitions))
        {
            if (definitions.TryGetValue(serviceType, out var definition)){
                return definition;
            }
            throw new KeyNotFoundException($"External Service Definition for '{environment}' - '{serviceType}' not found in dictionary.");
        }
        throw new KeyNotFoundException($"External Services configurations for '{environment}' not found in dictionary.");
    }
}
