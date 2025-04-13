using Clinic.Domain.Common;
using ErrorOr;

namespace Clinic.Domain;
public class Appointment : AggregateRoot
{
    public Appointment(DateTime appointmentDate,
    int appointmentDurationMinutes,
    Guid doctorId,
    Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        AppointmentDate = appointmentDate;
        AppointmentDurationMinutes = appointmentDurationMinutes;
        _doctorId = doctorId;
    }


    public Appointment()
    {

    }


    public DateTime AppointmentDate { get; }
    public int AppointmentDurationMinutes { get; }
    private readonly Guid _doctorId;



    public static ErrorOr<Appointment> Create(DateTime appointmentDate,
    int appointmentDurationMinutes,
    Guid _doctorId,
    Guid? id = null
    )
    {
        var validateAppointmentDateResult = ValidateAppointmentDate(appointmentDate);
        if (validateAppointmentDateResult.IsError)
            return validateAppointmentDateResult.Errors;

        return new Appointment(appointmentDate, appointmentDurationMinutes, _doctorId, id);
    }



    private static ErrorOr<Success> ValidateAppointmentDate(DateTime appointmentDate)
    {
        if (appointmentDate < DateTime.Now)
        {
            return Error.Validation(description: "Appointment date cannot be in the past.",
                code: AppointmentErrors.InvalidAppointmentDate);
        }


        // Validate that the day is between Saturday and Wednesday
        if (appointmentDate.DayOfWeek != DayOfWeek.Saturday &&
     appointmentDate.DayOfWeek != DayOfWeek.Sunday &&
     appointmentDate.DayOfWeek != DayOfWeek.Monday &&
     appointmentDate.DayOfWeek != DayOfWeek.Tuesday &&
     appointmentDate.DayOfWeek != DayOfWeek.Wednesday)
        {
            return Error.Validation(description: "Appointment date must be from Saturday to Wednesday.",
                code: AppointmentErrors.InvalidAppointmentDate);
        }

        // Validate that the time is between 9 AM and 6 PM
        var startTime = new TimeSpan(9, 0, 0); // 9:00 AM
        var endTime = new TimeSpan(18, 0, 0); // 6:00 PM
        if (appointmentDate.TimeOfDay < startTime || appointmentDate.TimeOfDay > endTime)
        {
            return Error.Validation(
                description: "Appointment date must be within working hours (9 AM to 6 PM).",
                code: AppointmentErrors.InvalidAppointmentDate);
        }
        return Result.Success;
    }
}
