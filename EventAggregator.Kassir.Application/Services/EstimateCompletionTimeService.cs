using EventAggregator.Kassir.Domain.Services;
using EventAggregator.Shared.ShowEntities.Enums;
using EventAggregator.Shared.ShowEntities.ValueObjects;

namespace EventAggregator.Kassir.Application.Services;

public class EstimateCompletionTimeService : IEstimateCompletionTimeService
{
    public int CalculateEstimatedCompletionTimeSecond(List<DateRange> searchDateRanges, List<ShowType> showTypes)
    {
        return searchDateRanges.Count * showTypes.Count * 3;//TODO Research and replace
    }
}
