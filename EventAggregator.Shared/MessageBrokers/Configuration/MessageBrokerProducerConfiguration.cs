namespace EventAggregator.Shared.MessageBrokers.Configuration;

public record MessageBrokerProducerConfiguration
{
    public bool? EnableIdempotence { get; init; }
    public bool? CheckSending { get; init; }
    public int? MessageSendMaxRetries { get; init; }
    public int? RetryBackoffMs { get; init; }
    public int? FlushTimeoutMs { get; init; }
}
