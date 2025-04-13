using Clinic.Domain.Common;
using ErrorOr;
using Clinic.Domain.DoctorAggregate;

namespace Clinic.Domain;

public class Doctor : Entity
{

    public Doctor()
    {

    }
    public Doctor(
        DoctorType? doctorType,
        Guid? id) : base(id ?? Guid.NewGuid())
    {
        _doctorType = doctorType ?? DoctorType.General;
    }

    private readonly DoctorType _doctorType;
    private readonly List<Guid> _appointments = new List<Guid>();
    private readonly List<Schedule> _weaklySchedules = new List<Schedule>();


    public ErrorOr<Success> AddAppointment(Appointment appointment)
    {
        if (_appointments.Contains(appointment.Id))
        {
            return Error.Validation(
                description: "The appointment already exists."
            );
        }

        //validate appointment duration
        var validateAppointmentDuration = ValidateAppointmentDuration(appointment.AppointmentDurationMinutes);
        if (validateAppointmentDuration.IsError)
            return validateAppointmentDuration.Errors;

        //validate schedule
        var isValidSchedule = IsValidSchedule(appointment.AppointmentDate);
        if (isValidSchedule.IsError)
            return isValidSchedule.Errors;


        _appointments.Add(appointment.Id);
        return Result.Success;
    }

    private ErrorOr<Success> ValidateAppointmentDuration(int appointmentDurationMinutes)
    {
        var validAppointmentDuration = GetValidAppointmentDuration();
        if (appointmentDurationMinutes < validAppointmentDuration.MinTime ||
            appointmentDurationMinutes > validAppointmentDuration.MaxTime)
        {
            return Error.Validation(
                description: $"Appointment duration must be between {validAppointmentDuration.MinTime} and {validAppointmentDuration.MaxTime} minutes.",
                code: AppointmentErrors.InvalidAppointmentDuration);
        }
        return Result.Success;
    }

    public TimeDuration GetValidAppointmentDuration()
    {
        switch (_doctorType)
        {
            case DoctorType.General:
                return TimeDuration.Create(5, 15);
            case DoctorType.Specialist:
                return TimeDuration.Create(10, 30);
            default:
                throw new ArgumentOutOfRangeException(nameof(_doctorType), _doctorType, null);
        }
    }

    public ErrorOr<Success> IsValidSchedule(DateTime appointmentDateTime)
    {
        var dayOfWeek = appointmentDateTime.DayOfWeek;
        var schedule = _weaklySchedules.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
        if (schedule==null || 
            schedule.TimeRange.StartTime > appointmentDateTime.TimeOfDay || 
            schedule.TimeRange.EndTime < appointmentDateTime.TimeOfDay)
        { 
            return Error.Validation(
                description: $"The schedule for {dayOfWeek} already exists.",
                code: DoctorErrors.InvalidSchedule);
        }
        return Result.Success;
    }

    public void AddSchedule(Schedule schedule)
    {
        _weaklySchedules.Add(schedule);
    }
}