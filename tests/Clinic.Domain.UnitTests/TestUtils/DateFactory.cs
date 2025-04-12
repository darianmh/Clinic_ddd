using System;

namespace Clinic.Domain.UnitTests.TestUtils
{
    public static class DateFactory
    {
        public static DateTime GenerateDate(
            int? addDays = 0,
            DayOfWeek? dayOfWeek = null,
            int? hour = null,
            int? minute = 0
        )
        {
            var dateTime = DateTime.Now;
            if (addDays.HasValue)
            {
                dateTime = dateTime.AddDays(addDays.Value);
            }
            if (dayOfWeek.HasValue)
            {
                dateTime = dateTime.AddDays((int)dayOfWeek - (int)dateTime.DayOfWeek);
            }
            if (hour.HasValue)
            {
                dateTime = dateTime.Date;
                dateTime = dateTime.AddHours(hour.Value);
            }
            if (minute.HasValue)
            {
                dateTime = dateTime.AddMinutes(minute.Value);
            }
            return dateTime;
        }
    }
}