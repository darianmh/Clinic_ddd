using Clinic.Domain.UnitTests.TestUtils;
using FluentAssertions;

namespace Clinic.Domain.UnitTests;

public class DoctorTests
{
    [Fact]
    public void AddAppointment_WhenAddingMultipleAppointments_ShouldReturnSuccess()
    {
        //Arrange
        var doctor = DoctorFactory.CreateDoctor();
        var appointment1 = AppointmentFactory.CreateAppointment();
        var appointment2 = AppointmentFactory.CreateAppointment();

        //Act
        var addAppointmentResult1 = doctor.AddAppointment(appointment1.Id);
        var addAppointmentResult2 = doctor.AddAppointment(appointment2.Id);

        //Assert
        addAppointmentResult1.IsError.Should().BeFalse();
        addAppointmentResult2.IsError.Should().BeFalse();
    }
}

