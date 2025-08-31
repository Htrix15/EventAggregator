using EventAggregator.Shared.ExternalServices.Enums;

namespace EventAggregator.Shared.MessageBrokers.Configuration;

public record MessageBrokerConfiguration
{
    public ExternalServiceType Client { get; set; }
    public required string BootstrapServers { get; init; }
}
