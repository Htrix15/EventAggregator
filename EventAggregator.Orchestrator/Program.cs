using EventAggregator.Shared.Infrastructure.Kafka.Consumer;
using EventAggregator.Shared.MessageBrokers.Enums;
using EventAggregator.Shared.ShowEntities.Messages;
using EventAggregator.Shared.WebApplications.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services
    .AddKafkaMessageToChannelConsumerServices<StartShowAggregationMessage, 
        KafkaConsumerMessageToChannelHandler<StartShowAggregationMessage>>
    (builder.Configuration.GetSection("MessageBroker"),
        builder.Configuration.GetSection("MessageBroker:Consumer"),
        GroupType.Orchestrator,
        TopicType.StartShowAggregation);
var app = builder.Build();

app.UseHttpsRedirection();

app.MapHealthChecks();

await app.StartKafkaBus();

app.Run();
