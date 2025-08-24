using EventAggregator.Shared.ExternalServices.Enums;
using EventAggregator.Shared.MessageBrokers.Enums;

namespace EventAggregator.Shared.MessageBrokers.Configuration;

public record MessageBrokerConfiguration
{
    public ExternalServiceType? Client { get; init; }
    public GroupType? Group { get; init; }
    public string? BootstrapServers { get; init; }
}
