using EventAggregator.Shared.ExternalServices.Enums;
using EventAggregator.Shared.ExternalServices.Extensions;
using EventAggregator.Shared.Infrastructure.Kafka.Producer;
using EventAggregator.Shared.Infrastructure.Services;
using EventAggregator.Shared.MessageBrokers.Configuration;
using EventAggregator.Shared.MessageBrokers.Enums;
using EventAggregator.Shared.ShowEntities.Messages;
using EventAggregator.WebApi.Application;
using EventAggregator.WebApi.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfiguration();

builder.Services.AddSharedExternalServiceInfrastructure();

builder.Services.AddSingleton<MessageBrokersDefaultConfigurations>();
builder.Services.AddKafkaProducerServices<StartShowAggregationMessage, KafkaProducer<StartShowAggregationMessage>>(
    builder.Configuration.GetSection("MessageBroker"),
    TopicType.StartShowAggregation,
    overlayMessageBrokerConfiguration: true);

builder.Services.AddApplication();
builder.Services.AddExternalServiceHttpClient(ExternalServiceType.Orchestrator);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
