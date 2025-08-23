using EventAggregator.Shared.ExternalServices.Configurations;
using EventAggregator.Shared.ExternalServices.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventAggregator.Shared.ExternalServices.Extensions;

public static class AddExternalServiceHttpClientExtensions
{
    public static IServiceCollection AddExternalServiceHttpClient(this IServiceCollection services, 
        ExternalServiceType externalServiceType)
    {
        services.AddHttpClient(externalServiceType.ToString(), (serviceProvider, client) =>
        {
            var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

            var environmentName = environment switch
            {
                _ when environment.IsDevelopment() => Environments.Development,
                _ when environment.IsStaging() => Environments.Staging,
                _ when environment.IsProduction() => Environments.Production,
                _ => throw new InvalidOperationException($"Unsupported environment: {environment.EnvironmentName}")
            };

            client.BaseAddress = new Uri(ExternalServicesDictionary.GetExternalServiceDefinition(environmentName, externalServiceType).BaseUri);
        });

        return services;
    }
}
