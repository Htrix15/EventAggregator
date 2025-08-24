using EventAggregator.Shared.MessageBrokers.Enums;

namespace EventAggregator.Shared.MessageBrokers.Configuration;

public record MessageBrokerConsumerConfiguration
{
    public required TopicType WorkCommandsTopic { get; init; }
    public TopicType? ControlCommandsTopic { get; init; }
}
