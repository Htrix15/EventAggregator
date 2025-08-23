using EventAggregator.Shared.ShowEntities.Enums;
using EventAggregator.Shared.ShowEntities.ValueObjects;

namespace EventAggregator.WebApi.Application.DTOs.Requests;

public record StartShowAggregationRequest
{
    public required List<DateRange> SearchDateRanges { get; set; }
    public required List<ShowType> ShowTypes { get; set; }
}
