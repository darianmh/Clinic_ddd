
using System.ComponentModel.DataAnnotations;

namespace Clinic.Domain.Common;

public class TimeRange : ValueObject
{
    private TimeRange(TimeSpan startTime, TimeSpan endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
    public TimeSpan StartTime { get; }
    public TimeSpan EndTime { get; }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }

    public static TimeRange Create(TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime >= endTime)
        {
            throw new ValidationException("Invalid time range.");
        }

        return new TimeRange(startTime, endTime);
    }
}
