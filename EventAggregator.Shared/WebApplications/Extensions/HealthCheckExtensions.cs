using EventAggregator.Shared.WebApplications.Constants;
using Microsoft.AspNetCore.Builder;

namespace EventAggregator.Shared.WebApplications.Extensions;

public static class HealthCheckExtensions
{
    public static void MapHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks(Endpoints.HealthCheckEndpoint);
    }
}
