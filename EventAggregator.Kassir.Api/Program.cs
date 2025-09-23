using EventAggregator.Kassir.Application;
using EventAggregator.Kassir.Infrastructure;
using EventAggregator.Shared.Infrastructure.Kafka.Consumer;
using EventAggregator.Shared.Infrastructure.Kafka.Producer;
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

builder.Services.AddKafkaProducer<SearchShowStartedMessage>(
    builder.Configuration.GetSection("MessageBroker:Settings"),
    builder.Configuration.GetSection("MessageBroker:Producers"),
    "SearchShowStarted");

builder.Services.AddKafkaProducer<ShowListMessage>(
    builder.Configuration.GetSection("MessageBroker:Settings"),
    builder.Configuration.GetSection("MessageBroker:Producers"),
    "ReturnShowList");

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapHealthChecks();

app.Run();
