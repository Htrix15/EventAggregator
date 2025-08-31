using EventAggregator.Shared.Infrastructure.Kafka.Consumer;
using EventAggregator.Shared.ShowEntities.Messages;
using EventAggregator.Shared.WebApplications.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafkaConsumer<StartShowAggregationMessage>(
    builder.Configuration.GetSection("MessageBroker:Settings"),
    builder.Configuration.GetSection("MessageBroker:Consumers"),
    "StartShowAggregation");

builder.Services.AddKafkaConsumer<BreakShowAggregationMessage>(
    builder.Configuration.GetSection("MessageBroker:Settings"),
    builder.Configuration.GetSection("MessageBroker:Consumers"),
    "BreakShowAggregation");

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapHealthChecks();

app.Run();
