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
        var addAppointmentResult1 = doctor.AddAppointment(appointment1.Value.Id);
        var addAppointmentResult2 = doctor.AddAppointment(appointment2.Value.Id);

        //Assert
        addAppointmentResult1.IsError.Should().BeFalse();
        addAppointmentResult2.IsError.Should().BeFalse();
    }

    [Fact]
    public void CreatingDoctor_ValidateAppointmentDuration()
    {
        //Arrange
        var generalDoctor = DoctorFactory.CreateDoctor(DoctorType.General);
        var specialistDoctor = DoctorFactory.CreateDoctor(DoctorType.Specialist);

        //Act
        var generalDoctorAppointmentDuration = generalDoctor.GetValidAppointmentDuration();
        var specialistDoctorAppointmentDuration = specialistDoctor.GetValidAppointmentDuration();

        //Assert
        generalDoctorAppointmentDuration.MaxTime.Should().BeLessThanOrEqualTo(15);
        generalDoctorAppointmentDuration.MinTime.Should().BeGreaterThanOrEqualTo(5);
        
        specialistDoctorAppointmentDuration.MaxTime.Should().BeLessThanOrEqualTo(30);
        specialistDoctorAppointmentDuration.MinTime.Should().BeGreaterThanOrEqualTo(10);
    }
}

