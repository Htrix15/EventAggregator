using EventAggregator.Shared.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace EventAggregator.Shared.MessageBrokers.Configuration;

public class MessageBrokersDefaultConfigurations(IWebHostEnvironment environment)
{
    private readonly string _environmentName = EnvironmentMapping.Map(environment.EnvironmentName);
    private readonly static Dictionary<string, MessageBrokerConfiguration> _configurations = new()
    {
        [Environments.Development] = new MessageBrokerConfiguration()
        {
            BootstrapServers = "localhost:9092",
            EnableIdempotence = false,
            CheckSending = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 100,
            FlushTimeoutMs = 10000
        }
    };

    public MessageBrokerConfiguration GetMessageBrokerConfiguration()
    {
        if (_configurations.TryGetValue(_environmentName, out var configuration))
        {
            return configuration;
        }
        throw new KeyNotFoundException($"Default Message Broker configurations for '{_environmentName}' not found in dictionary.");
    }
}
