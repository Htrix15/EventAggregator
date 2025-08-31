using EventAggregator.Shared.ExternalServices.Enums;
using EventAggregator.Shared.MessageBrokers.Enums;

namespace EventAggregator.Shared.MessageBrokers.Configuration;

public record MessageBrokerConfiguration
{
    public ExternalServiceType Client { get; set; }
    public GroupType Group { get; set; }
    public required string BootstrapServers { get; init; }
}
