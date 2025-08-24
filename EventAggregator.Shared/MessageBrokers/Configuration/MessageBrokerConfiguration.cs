using EventAggregator.Shared.ExternalServices.Enums;

namespace EventAggregator.Shared.MessageBrokers.Configuration;

public record MessageBrokerConfiguration
{
    public ExternalServiceType? ClientId { get; init; }
    public string? BootstrapServers { get; init; }
    public bool? EnableIdempotence { get; init; }
    public bool? CheckSending { get; init; }
    public int? MessageSendMaxRetries { get; init; }
    public int? RetryBackoffMs { get; init; }
    public int? FlushTimeoutMs { get; init; }
}
