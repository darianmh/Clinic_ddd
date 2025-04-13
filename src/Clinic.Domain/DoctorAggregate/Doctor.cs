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
    private readonly List<Appointment> _appointments = new List<Appointment>();
    private readonly List<Schedule> _weaklySchedules = new List<Schedule>();

    private List<Guid> AppointmentIds =>
        _appointments.Select(x => x.Id).ToList();


    public ErrorOr<Success> AddAppointment(Appointment appointment)
    {
        if (AppointmentIds.Contains(appointment.Id))
        {
            return Error.Validation(
                description: "The appointment already exists."
            );
        }

        //validate appointment duration
        var validateDurationMinutes = ValidateDurationMinutes((appointment.EndDateTime - appointment.StartDateTime).Minutes);
        if (validateDurationMinutes.IsError)
            return validateDurationMinutes.Errors;

        //validate schedule
        var isValidSchedule = IsValidSchedule(appointment.StartDateTime);
        if (isValidSchedule.IsError)
            return isValidSchedule.Errors;


        var overlappingResult = CheckMaximumOverlappingAppointments(appointment);
        if (overlappingResult.IsError)
            return overlappingResult.Errors;

        _appointments.Add(appointment);
        return Result.Success;
    }

    private ErrorOr<Success> CheckMaximumOverlappingAppointments(Appointment appointment)
    {
        var maxOverlappingAppointments = GetMaximumAllowedOverlappingAppointments();
        var overlappingAppointments = _appointments
            .Where(x => x.OverlapsDateTime(appointment));

        if (overlappingAppointments.Count() >= maxOverlappingAppointments)
        {
            return Error.Validation(
                description: $"The maximum number of overlapping appointments is {maxOverlappingAppointments}.",
                code: AppointmentErrors.AppointmentMaxOverlapExceeded);
        }
        return Result.Success;
    }

    private ErrorOr<Success> ValidateDurationMinutes(int DurationMinutesMinutes)
    {
        var validDurationMinutes = GetValidDurationMinutes();
        if (DurationMinutesMinutes < validDurationMinutes.MinTime ||
            DurationMinutesMinutes > validDurationMinutes.MaxTime)
        {
            return Error.Validation(
                description: $"Appointment duration must be between {validDurationMinutes.MinTime} and {validDurationMinutes.MaxTime} minutes.",
                code: AppointmentErrors.InvalidDurationMinutes);
        }
        return Result.Success;
    }

    public TimeDuration GetValidDurationMinutes()
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

    private uint GetMaximumAllowedOverlappingAppointments()
    {
        switch (_doctorType)
        {
            case DoctorType.General:
                return 2;
            case DoctorType.Specialist:
                return 3;
            default:
                throw new ArgumentOutOfRangeException(nameof(_doctorType), _doctorType, null);
        }
    }

    public ErrorOr<Success> IsValidSchedule(DateTime appointmentDateTime)
    {
        var dayOfWeek = appointmentDateTime.DayOfWeek;
        var schedule = _weaklySchedules.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
        if (schedule == null ||
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