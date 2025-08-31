using EventAggregator.Shared.MessageBrokers.Enums;

namespace EventAggregator.Shared.MessageBrokers.Configuration;

public record MessageBrokerConsumerConfiguration
{
    public int? BufferSize { get; init; }
    public int? WorkersCount { get; init; }
    public int? SessionTimeoutMs { get; init; }
    public int? MaxPollIntervalMs { get; init; }
    public int? HeartbeatIntervalMs { get; init; }
    public bool? EnableAutoCommit { get; init; }
    public bool? EnableAutoOffsetStore { get; init; }
    public AutoOffsetReset AutoOffsetReset { get; init; }
}
