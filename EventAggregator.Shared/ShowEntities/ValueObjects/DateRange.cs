namespace EventAggregator.Shared.ShowEntities.ValueObjects;

public readonly record struct DateRange
{
    public DateOnly Start { get; init; }
    public DateOnly End { get; init; }

    public DateRange(DateOnly start, DateOnly end)
    {
        if (start > end)
            throw new ArgumentException("Start date must be before end date");

        Start = start;
        End = end;
    }
}