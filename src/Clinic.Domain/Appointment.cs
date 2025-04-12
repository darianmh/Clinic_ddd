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
        if(appointmentDate.DayOfWeek < DayOfWeek.Saturday || appointmentDate.DayOfWeek > DayOfWeek.Wednesday)
        {
            return Error.Validation(description: "Appointment date must be from Saturday to Wednesday.");
        }
        if (appointmentDate.TimeOfDay.Hours < 9 || appointmentDate.TimeOfDay > new TimeSpan(18, 0, 0))
        {
            return Error.Validation(description: "Appointment date must be within working hours (9 AM to 6 PM).");
        }
        return Result.Success;
    }
}
