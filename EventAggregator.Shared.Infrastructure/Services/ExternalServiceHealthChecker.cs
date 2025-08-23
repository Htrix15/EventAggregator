using EventAggregator.Shared.ExternalServices.Services;

namespace EventAggregator.Shared.Infrastructure.Services;

public class ExternalServiceHealthChecker(IHttpClientFactory httpClientFactory) : IExternalServiceHealthChecker
{
    public async Task<bool> IsServiceHealthy(string serviceName, CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(serviceName);
            var response = await client.GetAsync(WebApplications.Constants.Endpoints.HealthCheckEndpoint, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
