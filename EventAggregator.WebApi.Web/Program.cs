using EventAggregator.Shared.ExternalServices.Enums;
using EventAggregator.Shared.ExternalServices.Extensions;
using EventAggregator.Shared.Infrastructure.Kafka.Producer;
using EventAggregator.Shared.Infrastructure.Services;
using EventAggregator.Shared.ShowEntities.Messages;
using EventAggregator.WebApi.Application;
using EventAggregator.WebApi.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfiguration();

builder.Services.AddSharedExternalServiceInfrastructure();

builder.Services.AddOptions();

builder.Services.AddKafkaProducer<StartShowAggregationMessage>(
    builder.Configuration.GetSection("MessageBroker:Settings"),
    builder.Configuration.GetSection("MessageBroker:Producers"),
    "StartShowAggregation");

builder.Services.AddKafkaProducer<BreakShowAggregationMessage>(
    builder.Configuration.GetSection("MessageBroker:Settings"),
    builder.Configuration.GetSection("MessageBroker:Producers"),
    "BreakShowAggregation");

builder.Services.AddExternalServiceHttpClient(ExternalServiceType.Orchestrator);

builder.Services.AddApplication();

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
