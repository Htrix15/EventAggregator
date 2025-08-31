using EventAggregator.Shared.Infrastructure.Kafka.Consumer;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Enums;
using EventAggregator.Shared.ShowEntities.Messages;
using EventAggregator.Shared.WebApplications.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<MessageBrokersDefaultConfigurations>();
builder.Services
    .AddKafkaMessageToChannelConsumerServices<StartShowAggregationMessage, 
        KafkaConsumerMessageToChannelHandler<StartShowAggregationMessage>>
    (builder.Configuration.GetSection("MessageBroker"),
        builder.Configuration.GetSection("MessageBroker"),
        GroupType.Orchestrator,
        TopicType.StartShowAggregation,
        overlayMessageBrokerConfiguration: true,
        overlayConsumerConfiguration: true);
var app = builder.Build();

app.UseHttpsRedirection();

app.MapHealthChecks();

await app.StartKafkaBus();

app.Run();
