using Clinic.Domain.UnitTests.TestUtils;
using FluentAssertions;

namespace Clinic.Domain.UnitTests;

public class AppointmentTests
{

    [Theory]
    [InlineData(-1, null, null, null)]//should be yesterday
    [InlineData(null, DayOfWeek.Monday, 7, null)] //should be next Monday at 7
    [InlineData(null, DayOfWeek.Friday, null, null)]//should be next Friday
    [InlineData(null, DayOfWeek.Sunday, 18, 3)]//should be next Sunday at 18:03
    public void Create_WhenCalledWithInValidDate_ShouldReturnError(int? addDays,
        DayOfWeek? dayOfWeek,
        int? hour,
        int? minute)
    {
        //Arrange
        var appointmentDate = DateFactory.GenerateDate(addDays, dayOfWeek, hour, minute);

        //Act
        var result = Appointment.Create(appointmentDate);

        //Assert
        result.IsError.Should().BeTrue();
        result.Value.Should().BeNull();
    }
    [Theory]
    [InlineData(null, DayOfWeek.Tuesday, 9, null)]//should be next Thursday at 8
    [InlineData(null, DayOfWeek.Monday, 12, null)]//should be next Monday at 12
    [InlineData(null, DayOfWeek.Sunday, 18, null)]//should be next Sunday at 18
    public void Create_WhenCalledWithValidDate_ShouldReturnAppointment(int? addDays,
        DayOfWeek? dayOfWeek,
        int? hour,
        int? minute)
    {
        //Arrange
        var appointmentDate = DateFactory.GenerateDate(addDays, dayOfWeek, hour, minute);

        //Act
        var result = Appointment.Create(appointmentDate);

        //Assert
        if (result.IsError)
            Console.WriteLine("Generated Date: " + appointmentDate.ToString("yyyy-MM-dd HH:mm:ss") + " Error: " + result.Errors.First().Description);
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
    }

}

