using EventAggregator.Shared.ShowEntities.Enums;
using EventAggregator.Shared.ValueObjects;

namespace EventAggregator.WebApi.Application.DTOs.Requests;

public class StartEventAggregationRequest
{
    public required List<DateRange> SearchDateRanges { get; set; }
    public required List<ShowType> ShowTypes { get; set; }
}
