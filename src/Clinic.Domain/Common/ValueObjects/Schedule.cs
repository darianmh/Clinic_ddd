using Clinic.Domain.Common;

namespace Clinic.Domain;

public class Schedule : ValueObject
{

    private Schedule(DayOfWeek dayOfWeek, TimeRange timeRange)
    {
        DayOfWeek = dayOfWeek;
        TimeRange = timeRange;
    }

    public DayOfWeek DayOfWeek { get; }
    public TimeRange TimeRange { get; }


    public static Schedule Create(DayOfWeek dayOfWeek, TimeRange timeRange)
    {
        return new Schedule(dayOfWeek, timeRange);
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return DayOfWeek;
        yield return TimeRange;
    }
}