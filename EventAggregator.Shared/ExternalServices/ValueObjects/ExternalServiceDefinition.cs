namespace EventAggregator.Shared.ExternalServices.ValueObjects;

public readonly record struct ExternalServiceDefinition
{
    public required string BaseUri { get; init; }
}
