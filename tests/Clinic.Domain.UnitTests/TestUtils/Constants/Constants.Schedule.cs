using Clinic.Domain.Common;

namespace Clinic.Domain.UnitTests.TestUtils.TestConstants;

public static partial class Constants
{
    public static class Schedule
    {
        public static readonly Domain.Schedule ValidSchedule =
            Domain.Schedule.Create(DayOfWeek.Monday, TimeRange.Create(new TimeSpan(hours: 11, minutes: 0, seconds: 0),
                new TimeSpan(hours: 15,minutes: 0, seconds: 0)));
    }
}