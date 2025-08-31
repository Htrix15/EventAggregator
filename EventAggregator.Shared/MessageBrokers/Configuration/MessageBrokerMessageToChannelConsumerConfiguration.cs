using System.Threading.Channels;

namespace EventAggregator.Shared.MessageBrokers.Configuration;

public record MessageBrokerMessageToChannelConsumerConfiguration : MessageBrokerConsumerConfiguration
{
    public bool UnboundedChannel { get; init; }
    public BoundedChannelFullMode ChannelFullMode { get; init; }
    public int? ChannelCapacity { get; init; }
}
