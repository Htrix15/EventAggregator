using EventAggregator.Kassir.Application.Commands.NotifySearchShowStarted;
using EventAggregator.Kassir.Application.Commands.ReturnShowList;
using EventAggregator.Kassir.Application.Commands.StartShowAggregation;
using EventAggregator.Kassir.Application.Services;
using EventAggregator.Kassir.Domain.Services;
using EventAggregator.Shared.Commands.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EventAggregator.Kassir.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IEstimateCompletionTimeService, EstimateCompletionTimeService>();

        services.AddTransient<ICommandHandler<NotifySearchShowStartedCommand>, NotifySearchShowStartedCommandHandler>();
        services.AddTransient<ICommandHandler<ReturnShowListCommand>, ReturnShowListCommandHandler>();
        services.AddTransient<ICommandHandler<StartShowAggregationCommand>, StartShowAggregationHandler>();

        services.AddTransient<IMessageManagerService, MessageManagerService>();
        return services;
    }
}