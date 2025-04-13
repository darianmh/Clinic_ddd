using Clinic.Domain.DoctorAggregate;
using Clinic.Domain.UnitTests.TestUtils;
using Clinic.Domain.UnitTests.TestUtils.Services;
using Clinic.Domain.UnitTests.TestUtils.TestConstants;
using FluentAssertions;

namespace Clinic.Domain.UnitTests;
public class DoctorTests
{
    [Fact]
    public void AddAppointment_WhenAddingMultipleAppointments_ShouldReturnSuccess()
    {
        //Arrange
        var doctor = DoctorFactory.CreateDoctor();
        doctor.AddSchedule(Constants.Schedule.ValidSchedule);
        var appointment1 = AppointmentFactory.CreateAppointment();
        var appointment2 = AppointmentFactory.CreateAppointment();

        //Act
        var addAppointmentResult1 = doctor.AddAppointment(appointment1.Value);
        var addAppointmentResult2 = doctor.AddAppointment(appointment2.Value);

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

    [Fact]
    public void AddAppointmentForGeneralDoctor_WhenMoreThanTwoAppointmentsWithOverlap_ShouldReturnError()
    {
        //Arrange
        var doctor = Constants.Doctor.GeneralDoctor;
        doctor.AddSchedule(Constants.Schedule.ValidSchedule);

        var appointment1 = AppointmentFactory.CreateAppointment(Constants.Date.ValidAppointmentDateTime,
            Constants.Appointment.ValidAppointmentDuration,
            doctor.Id);
        var appointment2 = AppointmentFactory.CreateAppointment(Constants.Date.ValidAppointmentDateTime,
            Constants.Appointment.ValidAppointmentDuration,
            doctor.Id);
        var appointment3 = AppointmentFactory.CreateAppointment(Constants.Date.ValidAppointmentDateTime,
            Constants.Appointment.ValidAppointmentDuration,
            doctor.Id);

        //Act
        var addAppointmentResult1 = doctor.AddAppointment(appointment1.Value);
        var addAppointmentResult2 = doctor.AddAppointment(appointment2.Value);
        var addAppointmentResult3 = doctor.AddAppointment(appointment3.Value);

        //Assert
        addAppointmentResult1.IsError.Should().BeFalse();
        addAppointmentResult2.IsError.Should().BeFalse();
        addAppointmentResult3.IsError.Should().BeTrue();
        addAppointmentResult3.FirstError.Code.Should().Be(AppointmentErrors.AppointmentMaxOverlapExceeded);

    }
    [Fact]
    public void AddAppointmentForSpecialistDoctor_WhenMoreThanThreeAppointmentsWithOverlap_ShouldReturnError()
    {
        //Arrange
        var doctor = Constants.Doctor.SpecialistDoctor;
        doctor.AddSchedule(Constants.Schedule.ValidSchedule);

        var appointment1 = AppointmentFactory.CreateAppointment(Constants.Date.ValidAppointmentDateTime,
            Constants.Appointment.ValidAppointmentDuration,
            doctor.Id);
        var appointment2 = AppointmentFactory.CreateAppointment(Constants.Date.ValidAppointmentDateTime,
            Constants.Appointment.ValidAppointmentDuration,
            doctor.Id);
        var appointment3 = AppointmentFactory.CreateAppointment(Constants.Date.ValidAppointmentDateTime,
            Constants.Appointment.ValidAppointmentDuration,
            doctor.Id);
        var appointment4 = AppointmentFactory.CreateAppointment(Constants.Date.ValidAppointmentDateTime,
            Constants.Appointment.ValidAppointmentDuration,
            doctor.Id);

        //Act
        var addAppointmentResult1 = doctor.AddAppointment(appointment1.Value);
        var addAppointmentResult2 = doctor.AddAppointment(appointment2.Value);
        var addAppointmentResult3 = doctor.AddAppointment(appointment3.Value);
        var addAppointmentResult4 = doctor.AddAppointment(appointment4.Value);

        //Assert
        addAppointmentResult1.IsError.Should().BeFalse();
        addAppointmentResult2.IsError.Should().BeFalse();
        addAppointmentResult3.IsError.Should().BeFalse();
        addAppointmentResult4.IsError.Should().BeTrue();
        addAppointmentResult4.FirstError.Code.Should().Be(AppointmentErrors.AppointmentMaxOverlapExceeded);

    }


    [Theory]
    [InlineData(DoctorType.General, 16)] // General doctor with 16 minutes duration
    [InlineData(DoctorType.Specialist, 9)] // Specialist doctor with 9 minutes duration
    [InlineData(DoctorType.General, 4)] // General doctor with 4 minutes duration
    [InlineData(DoctorType.Specialist, 31)] // Specialist doctor with 31 minutes duration
    public async Task AddAppointment_WhenInvalidDurationForDoctor_ShouldReturnError(
        DoctorType doctorType,
    uint appointmentDurationMinutes)
    {
        // Arrange
        var doctor = doctorType == DoctorType.General ? Constants.Doctor.GeneralDoctor : Constants.Doctor.SpecialistDoctor;
        var unitOfWork = new UnitOfWork();
        await unitOfWork.DoctorRepository.AddAsync(doctor);


        var appointmentResult = AppointmentFactory.CreateAppointment(Constants.Date.ValidAppointmentDateTime,
            appointmentDurationMinutes,
            doctor.Id);

        // Act
        var addAppointmentResult = doctor.AddAppointment(appointmentResult.Value);

        // Assert
        addAppointmentResult.IsError.Should().BeTrue();
        addAppointmentResult.FirstError.Code.Should().Be(AppointmentErrors.InvalidAppointmentDuration);
    }

    [Theory]
    [InlineData(DayOfWeek.Saturday, 12, 0)] // Invalid day and time (12:00 PM on Saturday)
    [InlineData(DayOfWeek.Tuesday, 18, 0)] // Invalid day and time (6:00 PM on Tuesday)
    [InlineData(DayOfWeek.Monday, 10, 59)] // Invalid day and time (10:59 PM on Monday)
    [InlineData(DayOfWeek.Monday, 15, 1)] // Invalid day and time (15:01 PM on Monday)
    public async Task AppAppointment_WhenAddingOutOfScheduleForDoctor_ShouldReturnError(DayOfWeek? dayOfWeek,
        int? hour,
        int? minute)
    {
        //Arrange
        var doctor = Constants.Doctor.GeneralDoctor;
        var unitOfWork = new UnitOfWork();
        await unitOfWork.DoctorRepository.AddAsync(doctor);
        var schedule = Constants.Schedule.ValidSchedule;
        doctor.AddSchedule(schedule);


        var appointmentResult = AppointmentFactory.CreateAppointment(DateFactory.GenerateDate(null, dayOfWeek, hour, minute),
            Constants.Appointment.ValidAppointmentDuration,
            doctor.Id);

        //Act
        var addAppointmentResult = doctor.AddAppointment(appointmentResult.Value);



        // Assert
        appointmentResult.IsError.Should().BeFalse();
        addAppointmentResult.IsError.Should().BeTrue();
        addAppointmentResult.FirstError.Code.Should().Be(DoctorErrors.InvalidSchedule);

    }
}

