using Clinic.Domain.Common;
using ErrorOr;

namespace Clinic.Domain;

public static class WorkingHours
{
    public static TimeSpan StartTime = new(9, 0, 0);
    public static TimeSpan EndTime = new(18, 0, 0);

    public static List<DayOfWeek> WorkingDays =
        [DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday];


    public static ErrorOr<Success> ValidateWorkingDate(DateTime date)
    {
        if (date < DateTime.Now)
        {
            return Error.Validation(description: "Date cannot be in the past.",
                code: WorkDateErrors.InvalidWorkingHour);
        }

        return ValidateWorkingDate(date.DayOfWeek, TimeRange.Create(date.TimeOfDay, date.TimeOfDay));
    }

    public static ErrorOr<Success> ValidateWorkingDate(DayOfWeek dayOfWeek, TimeRange timeRange)
    {
        // Validate that the day is between Saturday and Wednesday
        if (WorkingDays.All(wd => wd != dayOfWeek))
        {
            return Error.Validation(description: "Date must be from Saturday to Wednesday.",
                code: WorkDateErrors.InvalidWorkingHour);
        }

        // Validate that the time is between 9 AM and 6 PM
        var startTime = StartTime; // 9:00 AM
        var endTime = EndTime;// 6:00 PM
        if (timeRange.StartTime < startTime || timeRange.EndTime > endTime)
        {
            return Error.Validation(
                description: "Date must be within working hours (9 AM to 6 PM).",
                code: WorkDateErrors.InvalidWorkingHour);
        }
        return Result.Success;
    }
}