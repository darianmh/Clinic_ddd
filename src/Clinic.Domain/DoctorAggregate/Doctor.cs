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

    private ErrorOr<Success> ValidateDurationMinutes(int durationMinutes)
    {
        var validDurationMinutes = GetValidDurationMinutes();
        if (durationMinutes < validDurationMinutes.MinTime ||
            durationMinutes > validDurationMinutes.MaxTime)
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
                description: $"The doctor is not available on {dayOfWeek}.",
                code: DoctorErrors.InvalidSchedule);
        }
        return Result.Success;
    }

    public ErrorOr<Success> AddSchedule(Schedule schedule)
    {
        // Check if the schedule already exists
        if (_weaklySchedules.Any(x => x.DayOfWeek == schedule.DayOfWeek))
        {
            return Error.Validation(
                description: $"The schedule for {schedule.DayOfWeek} already exists.",
                code: DoctorErrors.DuplicatedSchedule);
        }

        // check if the schedule is in working hours
        if (WorkingHours.ValidateWorkingDate(schedule.DayOfWeek, schedule.TimeRange).IsError)
        {
            return Error.Validation(
                description: $"The schedule for {schedule.DayOfWeek} from {schedule.TimeRange.StartTime} to {schedule.TimeRange.EndTime} is not in working hours.",
                code: WorkDateErrors.InvalidWorkingHour);
        }

        _weaklySchedules.Add(schedule);
        return Result.Success;
    }

    // Finds the earliest available slot of the given duration
    public DateTime? FindEarliestAvailableAppointment(int durationMinutes, DateTime startSearchFrom)
    {
        // Check at least 4 weeks forward
        DateTime endSearchDate = startSearchFrom.AddDays(28);

        for (DateTime date = startSearchFrom.Date; date <= endSearchDate; date = date.AddDays(1))
        {
            // Check if this day is in the doctor's weekly schedule
            var schedule = _weaklySchedules.FirstOrDefault(ws => ws.DayOfWeek == date.DayOfWeek);
            if (schedule == null) continue;

            // Get available slots for this day
            var slots = GetAvailableTimeSlots(date, schedule, durationMinutes);

            // Return the earliest slot if any are found
            if (slots.Any())
            {
                return slots.First();
            }
        }

        return null; // No available slots found in the search period
    }

    // Gets all available time slots for a specific day
    private List<DateTime> GetAvailableTimeSlots(DateTime date, Schedule schedule, int durationMinutes)
    {
        List<DateTime> availableSlots = new List<DateTime>();

        // Create full working day timeline
        DateTime dayStart = date.Date + schedule.TimeRange.StartTime;
        DateTime dayEnd = date.Date + schedule.TimeRange.EndTime;

        // Adjust dayStart if searching for a slot in the current day and some time has already passed
        if (date.Date == DateTime.Today && DateTime.Now.TimeOfDay > schedule.TimeRange.StartTime)
        {
            // Round up to the nearest future slot
            dayStart = DateTime.Today + new TimeSpan(
                DateTime.Now.Hour,
                (DateTime.Now.Minute / 15 + 1) * 15,
                0);
        }

        // Get all appointments for this day
        var dayAppointments = _appointments
            .Where(a => a.StartDateTime.Date == date.Date)
            .OrderBy(a => a.StartDateTime)
            .ToList();

        // Find available slots by checking gaps between appointments
        DateTime currentTime = dayStart;

        foreach (var appointment in dayAppointments)
        {
            // Check if there's a gap before this appointment that can fit a slot
            if ((appointment.StartDateTime - currentTime).Minutes >= durationMinutes)
            {
                // Add all possible slots in this gap
                while (currentTime.AddMinutes(durationMinutes) <= appointment.StartDateTime)
                {
                    availableSlots.Add(currentTime);
                    currentTime = currentTime.AddMinutes(15); // Assuming 15-minute increments
                }
            }

            // Move current time to after this appointment
            currentTime = appointment.EndDateTime;
        }

        // Check for slots after the last appointment until the end of the working day
        while (currentTime.AddMinutes(durationMinutes) <= dayEnd)
        {
            availableSlots.Add(currentTime);
            currentTime = currentTime.AddMinutes(15);
        }

        return availableSlots;
    }
}