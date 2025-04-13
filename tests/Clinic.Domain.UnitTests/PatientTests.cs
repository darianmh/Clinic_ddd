using Clinic.Domain.PatientAggregae;
using Clinic.Domain.UnitTests.TestUtils;
using Clinic.Domain.UnitTests.TestUtils.TestConstants;
using FluentAssertions;

namespace Clinic.Domain.UnitTests;

public class PatientTests
{
    [Fact]
    public void AddAppointment_WhenAddingAppointmentWithOverlap_ShouldReturnError()
    {
        //Arrange
        var patient = PatientFactory.CreatePatient();
        var appointmentDate = DateTime.Now.AddDays(1);
        var appointmentDuration = Constants.Appointment.ValidAppointmentDuration;

        var appointment1 = AppointmentFactory.CreateAppointment(
            appointmentDate,
            appointmentDuration);
        var appointment2 = AppointmentFactory.CreateAppointment(
            appointmentDate.AddMinutes(2),
            appointmentDuration);

        //Act
        var addAppointmentResult1 = patient.Value.AddAppointment(appointment1.Value);
        var addAppointmentResult2 = patient.Value.AddAppointment(appointment2.Value);

        //Assert
        addAppointmentResult1.IsError.Should().BeFalse();
        addAppointmentResult2.IsError.Should().BeTrue();
        addAppointmentResult2.FirstError.Code.Should().Be(PatientErrors.AppointmentOverlap);
    }

    [Fact]
    public void AddAppointment_WhenAddingMoreThanTwoAppointment_ShouldReturnError()
    {
        //Arrange
        var patient = PatientFactory.CreatePatient();
        var appointmentDate = DateTime.Now.AddDays(1);
        var appointmentDuration = Constants.Appointment.ValidAppointmentDuration;

        var appointment1 = AppointmentFactory.CreateAppointment(
            appointmentDate,
            appointmentDuration);
        var appointment2 = AppointmentFactory.CreateAppointment(
            appointmentDate.AddMinutes(appointmentDuration),
            appointmentDuration);
        var appointment3 = AppointmentFactory.CreateAppointment(
            appointmentDate.AddMinutes(appointmentDuration).AddMinutes(appointmentDuration),
            appointmentDuration);

        //Act
        var addAppointmentResult1 = patient.Value.AddAppointment(appointment1.Value);
        var addAppointmentResult2 = patient.Value.AddAppointment(appointment2.Value);
        var addAppointmentResult3 = patient.Value.AddAppointment(appointment3.Value);

        //Assert
        addAppointmentResult1.IsError.Should().BeFalse();
        addAppointmentResult2.IsError.Should().BeFalse();
        addAppointmentResult3.IsError.Should().BeTrue();
        addAppointmentResult3.FirstError.Code.Should().Be(PatientErrors.AppointmentCountExceeded);
    }
}