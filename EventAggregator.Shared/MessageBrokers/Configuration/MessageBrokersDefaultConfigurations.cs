using EventAggregator.Shared.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
}
