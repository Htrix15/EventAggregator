using EventAggregator.WebApi.Web.Configuration.Swagger;
using System.Text.Json.Serialization;

namespace EventAggregator.WebApi.Web.Configuration;

public static class ServiceCollectionExtensions
{
    public static void AddApiConfiguration(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerConfiguration();
    }
}
