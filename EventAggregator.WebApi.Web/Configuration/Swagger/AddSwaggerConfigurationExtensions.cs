using EventAggregator.WebApi.Web.Configuration.Swagger.Filters;
using Microsoft.OpenApi.Models;

namespace EventAggregator.WebApi.Web.Configuration.Swagger;

public static class AddSwaggerConfigurationExtensions
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Event Aggregator API",
                Version = "v1",
                Description = "API for event aggregation service"
            });

            c.SchemaFilter<DateOnlySchemaFilter>();
        });
    }
}
