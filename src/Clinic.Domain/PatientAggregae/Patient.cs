using Clinic.Domain.Common;
using ErrorOr;

namespace Clinic.Domain.PatientAggregae;

public class Patient : Entity
{
    private const int MaxDailyAppointments = 2;
    private readonly List<Appointment> _appointments = new();
    private Dictionary<DateOnly, List<Appointment>> AppointmentsByDate =>
        _appointments.GroupBy(a => a.AppointmentStartDate.Date)
            .ToDictionary(g => new DateOnly(g.Key.Year, g.Key.Month, g.Key.Day), g => g.ToList());

    private Patient(Guid? id = null) : base(id ?? Guid.NewGuid())
    {

    }

    public static ErrorOr<Patient> Create(Guid? id = null)
    {
        var patient = new Patient(id ?? Guid.NewGuid());
        return patient;
    }

    public ErrorOr<Success> AddAppointment(Appointment appointment)
    {

        var appointmentDateOnly = new DateOnly(appointment.AppointmentStartDate.Year,
            appointment.AppointmentStartDate.Month, appointment.AppointmentStartDate.Day);

        AppointmentsByDate.TryGetValue(appointmentDateOnly, out var appointmentsByDate);
        appointmentsByDate ??= new List<Appointment>();


        if (appointmentsByDate.Any(a => a.OverlapsDateTime(appointment)))
        {
            return Error.Conflict(description: "Patient already has an appointment at that time",
                code: PatientErrors.AppointmentOverlap);
        }
        if (appointmentsByDate.Count >= MaxDailyAppointments)
        {
            return Error.Conflict(description: $"Patient cannot have more than {MaxDailyAppointments} appointments per day",
                code: PatientErrors.AppointmentCountExceeded);
        }


        _appointments.Add(appointment);
        return Result.Success;
    }
}