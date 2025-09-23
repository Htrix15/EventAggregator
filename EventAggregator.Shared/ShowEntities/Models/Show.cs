using EventAggregator.Shared.ShowEntities.Enums;

namespace EventAggregator.Shared.ShowEntities.Models;

public record Show
{
    public required Billboard Billboard { get; init; }
    public required ShowType Type { get; init; }
    public required List<DateTime> Dates { get; init; }
    public required string? Title { get; init; }
    public required string? ImageLink { get; init; }
    public required string? Place { get; init; }
    public required string? Link { get; init; }
}
