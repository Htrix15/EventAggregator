using EventAggregator.Shared.ExternalServices.Enums;
using EventAggregator.Shared.ExternalServices.Extensions;
using EventAggregator.Shared.Infrastructure;
using EventAggregator.WebApi.Application;
using EventAggregator.WebApi.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfiguration();

builder.Services.AddSharedInfrastructure();
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
