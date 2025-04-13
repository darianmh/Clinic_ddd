namespace Clinic.Domain.UnitTests.TestUtils.TestConstants;

public static partial class Constants
{
    public static class Date
    {
        public static DateTime ValidAppointmentDateTime =
            DateFactory.GenerateDate(addDays: null, dayOfWeek: DayOfWeek.Monday, hour: 12, minute: null);
    }
}