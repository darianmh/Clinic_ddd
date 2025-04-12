using Clinic.Domain.UnitTests.TestUtils;
using FluentAssertions;

namespace Clinic.Domain.UnitTests;
public class DateFactoryTests
{
    [Theory]
    [InlineData(-1, null, null, null)]//should be yesterday
    [InlineData(null, DayOfWeek.Monday, 7, null)] //should be next Monday at 7
    [InlineData(null, DayOfWeek.Friday, null, null)]//should be next Friday
    [InlineData(null, DayOfWeek.Thursday, 8, null)]//should be next Thursday at 8
    [InlineData(null, DayOfWeek.Monday, 12, null)]//should be next Monday at 12
    [InlineData(null, DayOfWeek.Sunday, 18, null)]//should be next Sunday at 18
    [InlineData(null, DayOfWeek.Sunday, 18, 3)]//should be next Sunday at 18:03
    public void GenerateDate_ShouldReturnDateTime(int? addDays,
        DayOfWeek? dayOfWeek,
        int? hour,
        int? minute
  )
    {
        // Arrange


        // Act
        var result = DateFactory.GenerateDate(addDays, dayOfWeek, hour, minute);

        // Assert

        if (addDays.HasValue)
        {
            result.Date.Should().Be(DateTime.Now.AddDays(addDays.Value).Date);
        }
        if (dayOfWeek.HasValue)
        {
            result.DayOfWeek.Should().Be(dayOfWeek.Value);
        }
        if (hour.HasValue)
        {
            result.Hour.Should().Be(hour.Value);
        }
    }
}