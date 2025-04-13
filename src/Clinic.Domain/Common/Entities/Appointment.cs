using Clinic.Domain.Common;
using ErrorOr;

namespace Clinic.Domain;
public class Appointment : AggregateRoot
{
    public Appointment(DateTime startDateTime,
        DateTime endDateTime,
        Guid doctorId,
        Guid patientId,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        _doctorId = doctorId;
        _patientId = patientId;
    }


    public Appointment(Guid patientId)
    {
        _patientId = patientId;
    }


    public DateTime StartDateTime { get; }
    public DateTime EndDateTime { get; }
    private readonly Guid? _doctorId;
    private readonly Guid? _patientId;



    public static ErrorOr<Appointment> Create(DateTime startDateTime,
    uint appointmentDurationMinutes,
    Guid doctorId,
    Guid patientId,
    Guid? id = null
    )
    {
        var validateAppointmentDateResult = ValidateAppointmentDate(startDateTime);
        if (validateAppointmentDateResult.IsError)
            return validateAppointmentDateResult.Errors;

        var appointmentEndDate = startDateTime.AddMinutes(appointmentDurationMinutes);
        return new Appointment(startDateTime, appointmentEndDate, doctorId, patientId, id);
    }


    public bool OverlapsDateTime(Appointment other)
    {
        return (StartDateTime.Date == other.StartDateTime.Date
                && StartDateTime <= other.StartDateTime
                && EndDateTime > other.StartDateTime)
               ||
               (EndDateTime.Date == other.EndDateTime.Date
                && StartDateTime < other.EndDateTime
                && EndDateTime >= other.EndDateTime);
    }

    private static ErrorOr<Success> ValidateAppointmentDate(DateTime appointmentDate)
    {
        if (appointmentDate < DateTime.Now)
        {
            return Error.Validation(description: "Appointment date cannot be in the past.",
                code: AppointmentErrors.InvalidDurationMinutes);
        }


        // Validate that the day is between Saturday and Wednesday
        if (appointmentDate.DayOfWeek != DayOfWeek.Saturday &&
     appointmentDate.DayOfWeek != DayOfWeek.Sunday &&
     appointmentDate.DayOfWeek != DayOfWeek.Monday &&
     appointmentDate.DayOfWeek != DayOfWeek.Tuesday &&
     appointmentDate.DayOfWeek != DayOfWeek.Wednesday)
        {
            return Error.Validation(description: "Appointment date must be from Saturday to Wednesday.",
                code: AppointmentErrors.InvalidDurationMinutes);
        }

        // Validate that the time is between 9 AM and 6 PM
        var startTime = new TimeSpan(9, 0, 0); // 9:00 AM
        var endTime = new TimeSpan(18, 0, 0); // 6:00 PM
        if (appointmentDate.TimeOfDay < startTime || appointmentDate.TimeOfDay > endTime)
        {
            return Error.Validation(
                description: "Appointment date must be within working hours (9 AM to 6 PM).",
                code: AppointmentErrors.InvalidDurationMinutes);
        }
        return Result.Success;
    }
}
