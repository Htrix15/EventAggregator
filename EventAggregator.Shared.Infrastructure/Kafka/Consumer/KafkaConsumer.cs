using Confluent.Kafka;
using EventAggregator.Shared.MessageBrokers.Abstractions;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Constants;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Channels;

namespace EventAggregator.Shared.Infrastructure.Kafka.Consumer;

public class KafkaConsumer<TWorkCommandsMessage, TControlCommandsMessage> : BackgroundService, IDisposable
    where TWorkCommandsMessage : IMessage
    where TControlCommandsMessage : IMessage
{

    private readonly Channel<TWorkCommandsMessage> _workCommandsChannel;
    private readonly Channel<TControlCommandsMessage> _controlCommandsChannel;

    private readonly IConsumer<Ignore, JsonElement> _consumer;

    private readonly string _workCommandsTopic;
    private readonly string? _controlCommandsTopic;


    public KafkaConsumer(IOptions<MessageBrokerConfiguration> messageBrokerConfiguration,
        IOptions<MessageBrokerConsumerConfiguration> messageBrokerConsumerConfiguration,
        MessageBrokersDefaultConfigurations defaultConfigurationDictionary,
        Channel<TWorkCommandsMessage> workCommandsChannel,
        Channel<TControlCommandsMessage> controlCommandsChannel
        )
    {
        var messageBrokerOptions = messageBrokerConfiguration.Value;
        var messageBrokerConsumerOptions = messageBrokerConsumerConfiguration.Value;
        var defaultOptions = defaultConfigurationDictionary.GetMessageBrokerConfiguration();

        var consumerConfig = new ConsumerConfig()
        {
            ClientId = messageBrokerOptions.Client?.ToString()
               ?? defaultOptions.Client?.ToString()
               ?? throw new InvalidOperationException("MessageBrokerConfiguration - ClientId not definition"),

            GroupId = messageBrokerOptions.Group?.ToString()
               ?? defaultOptions.Group?.ToString()
               ?? throw new InvalidOperationException("MessageBrokerConfiguration - GroupId not definition"),

            BootstrapServers = messageBrokerOptions.BootstrapServers?.ToString()
               ?? defaultOptions.BootstrapServers?.ToString()
               ?? throw new InvalidOperationException("MessageBrokerConfiguration - BootstrapServers not definition"),

            EnableAutoCommit = false
        };

        _workCommandsTopic = Topics.GetTopic(messageBrokerConsumerOptions.WorkCommandsTopic);

        _controlCommandsTopic = messageBrokerConsumerOptions.ControlCommandsTopic.HasValue 
            ? Topics.GetTopic(messageBrokerConsumerOptions.ControlCommandsTopic.Value) 
            : null;

        _workCommandsChannel = workCommandsChannel;
        _controlCommandsChannel = controlCommandsChannel;

        _consumer = new ConsumerBuilder<Ignore, JsonElement>(consumerConfig).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        List<string> listeningTolics = [_workCommandsTopic];

        if (_controlCommandsTopic != null)
        {
            listeningTolics.Add(_controlCommandsTopic!);
        }

        _consumer.Subscribe(listeningTolics);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(cancellationToken);
                if (result == null) continue;

                await ProcessMessage(result, _consumer, cancellationToken);
            }
        }
        finally
        {
            _consumer.Close();
        }
    }

    private async Task ProcessMessage(ConsumeResult<Ignore, JsonElement> result,
        IConsumer<Ignore, JsonElement> consumer,
        CancellationToken сancellationToken)
    {
        try
        {
            if (_controlCommandsTopic != null && result.Topic == _controlCommandsTopic)
            {
                var controlCommand = JsonSerializer.Deserialize<TControlCommandsMessage>(result.Message.Value);
                await _controlCommandsChannel.Writer.WriteAsync(controlCommand!, сancellationToken);
                consumer.Commit(result);
            }
            else if (result.Topic == _workCommandsTopic)
            {
                var workCommand = JsonSerializer.Deserialize<TWorkCommandsMessage>(result.Message.Value);
                await _workCommandsChannel.Writer.WriteAsync(workCommand!, сancellationToken);
                consumer.Commit(result);
            }
        }
        catch (Exception ex)
        {
           //TODO Add loging
        }
    }

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
    }
}
