using EventAggregator.Shared.MessageBrokers.Enums;
using System.Threading.Channels;

namespace EventAggregator.Shared.MessageBrokers.Configuration;

public record MessageBrokerConsumerToChannelConfiguration
{
    public TopicType Topic { get; init; }
    public GroupType Group { get; init; }
    public required ChannelConfiguration ChannelConfiguration { get; init; }
}


public record ChannelConfiguration
{
    public bool Unbounded { get; init; }
    public BoundedChannelFullMode FullMode { get; init; }
    public int Capacity { get; init; }
}