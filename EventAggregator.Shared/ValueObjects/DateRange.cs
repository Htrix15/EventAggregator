namespace EventAggregator.Shared.ValueObjects;

public readonly struct DateRange
{
    public DateOnly Start { get; }
    public DateOnly End { get; }

    public DateRange(DateOnly start, DateOnly end)
    {
        if (start > end)
            throw new ArgumentException("Start date must be before end date");

        Start = start;
        End = end;
    }
}