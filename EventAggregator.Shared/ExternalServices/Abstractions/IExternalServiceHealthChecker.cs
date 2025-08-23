namespace EventAggregator.Shared.ExternalServices.Services;

public interface IExternalServiceHealthChecker
{
    Task<bool> IsServiceHealthy(string serviceName, CancellationToken cancellationToken);
}
