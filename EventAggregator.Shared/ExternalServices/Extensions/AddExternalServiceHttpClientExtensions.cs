using EventAggregator.Shared.ExternalServices.Configurations;
using EventAggregator.Shared.ExternalServices.Enums;
using EventAggregator.Shared.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace EventAggregator.Shared.ExternalServices.Extensions;

public static class AddExternalServiceHttpClientExtensions
{
    public static IServiceCollection AddExternalServiceHttpClient(this IServiceCollection services, 
        ExternalServiceType externalServiceType)
    {
        services.AddHttpClient(externalServiceType.ToString(), (serviceProvider, client) =>
        {
            var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

            var environmentName = EnvironmentMapping.Map(environment.EnvironmentName);

            client.BaseAddress = new Uri(ExternalServicesConfigurations.GetExternalServiceDefinition(environmentName, externalServiceType).BaseUri);
        });

        return services;
    }
}
