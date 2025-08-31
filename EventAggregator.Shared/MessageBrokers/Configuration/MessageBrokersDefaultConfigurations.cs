using EventAggregator.Shared.Mappers;
using EventAggregator.Shared.MessageBrokers.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;

namespace EventAggregator.Shared.MessageBrokers.Configuration;

public class MessageBrokersDefaultConfigurations(IWebHostEnvironment environment)
{
    private readonly string _environmentName = EnvironmentMapping.Map(environment.EnvironmentName);
    private readonly static Dictionary<string, MessageBrokerConfiguration> _messageBrokerConfigurations = new()
    {
        [Environments.Development] = new MessageBrokerConfiguration()
        {
            BootstrapServers = "localhost:9092",
        }
    };

    private readonly static Dictionary<string, MessageBrokerProducerConfiguration> _messageBrokerProducerConfiguration = new()
    {
        [Environments.Development] = new MessageBrokerProducerConfiguration()
        {
            EnableIdempotence = false,
            CheckSending = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 100,
            FlushTimeoutMs = 10000
        }
    };

    private readonly static Dictionary<string, MessageBrokerMessageToChannelConsumerConfiguration> _messageBrokerMessageToChannelConsumerConfiguration = new()
    {
        [Environments.Development] = new MessageBrokerMessageToChannelConsumerConfiguration()
        {
            BufferSize = 100,
            WorkersCount = 10,
            ChannelFullMode = BoundedChannelFullMode.Wait,
            UnboundedChannel = false,
            ChannelCapacity = 10,
            SessionTimeoutMs = 45000,
            MaxPollIntervalMs = 300000,
            HeartbeatIntervalMs = 3000,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false
        }
    };

    public MessageBrokerConfiguration GetMessageBrokerConfiguration()
    {
        if (_messageBrokerConfigurations.TryGetValue(_environmentName, out var configuration))
        {
            return configuration;
        }
        throw new KeyNotFoundException($"Default Message Broker configurations for '{_environmentName}' not found in dictionary.");
    }

    public MessageBrokerProducerConfiguration GetMessageBrokerProducerConfiguration()
    {
        if (_messageBrokerProducerConfiguration.TryGetValue(_environmentName, out var configuration))
        {
            return configuration;
        }
        throw new KeyNotFoundException($"Default Message Broker Producer configurations for '{_environmentName}' not found in dictionary.");
    }

    public MessageBrokerMessageToChannelConsumerConfiguration GetMessageBrokerMessageToChannelConsumerConfiguration()
    {
        if (_messageBrokerMessageToChannelConsumerConfiguration.TryGetValue(_environmentName, out var configuration))
        {
            return configuration;
        }
        throw new KeyNotFoundException($"Default Message Broker Message to Channel Consumer configurations for '{_environmentName}' not found in dictionary.");
    }
}
