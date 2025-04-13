using Clinic.Application.Appointments.Command.CreateAppointment;
using Clinic.Domain.UnitTests.TestUtils;
using Clinic.Domain.UnitTests.TestUtils.Services;
using Clinic.Domain.UnitTests.TestUtils.TestConstants;
using FluentAssertions;

namespace Clinic.Domain.UnitTests;

public class AppointmentTests
{
    [Theory]
    [InlineData(-1, null, null, null)] // Past date
    [InlineData(null, DayOfWeek.Friday, 10, null)] // Invalid day (Friday)
    [InlineData(null, DayOfWeek.Thursday, 10, null)] // Invalid day (Thursday)
    [InlineData(null, DayOfWeek.Monday, 8, 59)] // Invalid time (8:59 AM)
    [InlineData(null, DayOfWeek.Monday, 18, 1)] // Invalid time (6:01 PM)
    public void Create_WhenCalledWithInvalidDate_ShouldReturnError(
        int? addDays,
        DayOfWeek? dayOfWeek,
        int? hour,
        int? minute)
    {
        // Arrange
        var appointmentDate = DateFactory.GenerateDate(addDays, dayOfWeek, hour, minute);

        // Act
        var result = AppointmentFactory.CreateAppointment(appointmentDate);

        // Assert
        result.IsError.Should().BeTrue();
        result.Value.Should().BeNull();
        result.FirstError.Code.Should().Be(AppointmentError.InvalidAppointmentDate);
    }

    [Theory]
    [InlineData(null, DayOfWeek.Saturday, 9, 0)] // Valid day and time (9:00 AM on Saturday)
    [InlineData(null, DayOfWeek.Wednesday, 18, 0)] // Valid day and time (6:00 PM on Wednesday)
    [InlineData(null, DayOfWeek.Monday, 12, 30)] // Valid day and time (12:30 PM on Monday)
    public void Create_WhenCalledWithValidDate_ShouldReturnAppointment(
        int? addDays,
        DayOfWeek? dayOfWeek,
        int? hour,
        int? minute)
    {
        // Arrange
        var appointmentDate = DateFactory.GenerateDate(addDays, dayOfWeek, hour, minute);

        // Act
        var result = AppointmentFactory.CreateAppointment(appointmentDate);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
    }


    [Theory]
    [InlineData(DoctorType.General, 16)] // General doctor with 16 minutes duration
    [InlineData(DoctorType.Specialist, 9)] // Specialist doctor with 9 minutes duration
    [InlineData(DoctorType.General, 4)] // General doctor with 4 minutes duration
    [InlineData(DoctorType.Specialist, 31)] // Specialist doctor with 31 minutes duration
    public async Task AddAppointment_WhenInvalidDurationForDoctor_ShouldReturnError(
        DoctorType doctorType,
    int appointmentDurationMinutes)
    {
        // Arrange
        var doctor = doctorType == DoctorType.General ? Constants.Doctor.GeneralDoctor : Constants.Doctor.SpecialistDoctor;
        var doctorRepository = new DoctorRepository();
        var appointmentRepository = new AppointmentRepository();
        await doctorRepository.AddAsync(doctor);

        var CreateAppointmentHandler = new CreateAppointmentCommandHandler(appointmentRepository, doctorRepository);

        // Act

        var appointmentResult = await CreateAppointmentHandler.Handle(new CreateAppointmentCommand(
Constants.Date.ValidAppointmentDateTime,
appointmentDurationMinutes,
doctor.Id),
CancellationToken.None);

        // Assert
        appointmentResult.IsError.Should().BeTrue();
        appointmentResult.FirstError.Code.Should().Be(AppointmentError.InvalidAppointmentDuration);
    }
}

