using Clinic.Domain.Common;
using ErrorOr;

namespace Clinic.Domain;

public class Appointment : Entity
{
    public Appointment(DateTime appointmentDate,
    Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        _appointmentDate = appointmentDate;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="Appointment"/> class with a new unique identifier.
    /// </summary>

    public Appointment()
    {

    }


    private readonly DateTime _appointmentDate;



    public static ErrorOr<Appointment> Create(DateTime appointmentDate)
    {
        var ValidateAppointmentDateResult = ValidateAppointmentDate(appointmentDate);
        if (ValidateAppointmentDateResult.IsError)
            return ValidateAppointmentDateResult.Errors;

        return new Appointment(appointmentDate);
    }

    private static ErrorOr<Success> ValidateAppointmentDate(DateTime appointmentDate)
    {
        if (appointmentDate < DateTime.Now)
        {
            return Error.Validation(description: "Appointment date cannot be in the past.");
        }


        // Validate that the day is between Saturday and Wednesday
        if (appointmentDate.DayOfWeek != DayOfWeek.Saturday &&
     appointmentDate.DayOfWeek != DayOfWeek.Sunday &&
     appointmentDate.DayOfWeek != DayOfWeek.Monday &&
     appointmentDate.DayOfWeek != DayOfWeek.Tuesday &&
     appointmentDate.DayOfWeek != DayOfWeek.Wednesday)
        {
            return Error.Validation(description: "Appointment date must be from Saturday to Wednesday.");
        }

        // Validate that the time is between 9 AM and 6 PM
        var startTime = new TimeSpan(9, 0, 0); // 9:00 AM
        var endTime = new TimeSpan(18, 0, 0); // 6:00 PM
        if (appointmentDate.TimeOfDay < startTime || appointmentDate.TimeOfDay > endTime)
        {
            return Error.Validation(description: "Appointment date must be within working hours (9 AM to 6 PM).");
        }
        return Result.Success;
    }
}
