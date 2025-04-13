
using System.ComponentModel.DataAnnotations;
using ErrorOr;

namespace Clinic.Domain.Common;

public class TimeDuration : ValueObject
{
    private TimeDuration(int minTime, int maxTime)
    {
        MinTime = minTime;
        MaxTime = maxTime;
    }
    public int MinTime { get; }
    public int MaxTime { get; }


    public static TimeDuration Create(int minTime, int maxTime)
    {
        if (minTime < 0 || maxTime > 60 || minTime >= maxTime)
        {
            throw new ValidationException("Invalid time duration.");
        }

        return new TimeDuration(minTime, maxTime);
    }
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return MinTime;
        yield return MaxTime;
    }
}