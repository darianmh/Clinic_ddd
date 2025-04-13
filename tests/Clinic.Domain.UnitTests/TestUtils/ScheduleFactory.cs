using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinic.Domain.Common;

namespace Clinic.Domain.UnitTests.TestUtils;

public static class ScheduleFactory
{
    public static Schedule CreateSchedule(DayOfWeek dayOfWeek, TimeRange timeRange)
    {
        return Schedule.Create(dayOfWeek, timeRange);
    }
}