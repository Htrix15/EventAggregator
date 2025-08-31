using EventAggregator.Shared.Commands.Abstractions;
using EventAggregator.WebApi.Application.Commands.BreakShowAggregation;
using EventAggregator.WebApi.Application.Commands.StartShowAggregation;
using Microsoft.Extensions.DependencyInjection;


namespace EventAggregator.WebApi.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<ICommandHandler<StartShowAggregationCommand>, StartShowAggregationCommandHandler>();
        services.AddTransient<ICommandHandler<BreakShowAggregationCommand>, BreakShowAggregationCommandHandler>();
        return services;
    }
}
