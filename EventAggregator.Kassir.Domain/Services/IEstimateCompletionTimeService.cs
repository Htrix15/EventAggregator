using EventAggregator.Shared.ShowEntities.Enums;
using EventAggregator.Shared.ShowEntities.ValueObjects;

namespace EventAggregator.Kassir.Domain.Services;

public interface IEstimateCompletionTimeService
{
    public int CalculateEstimatedCompletionTimeSecond(List<DateRange> searchDateRanges, List<ShowType> showTypes);
}
