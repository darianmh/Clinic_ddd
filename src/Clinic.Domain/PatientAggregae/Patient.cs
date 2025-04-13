using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinic.Domain.Common;
using ErrorOr;

namespace Clinic.Domain.PatientAggregae;

public static class PatientErrors
{
    public const string AppointmentOverlap = "Patient.AppointmentOverlap";
    public const string AppointmentCountExceeded = "Patient.AppointmentCountExceeded";
}
public class PatientAppointment : ValueObject
{
    private PatientAppointment(Guid appointmentId,
        DateTime startDateTime,
        DateTime endDateTime)
    {
        AppointmentId = appointmentId;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
    }

    public Guid AppointmentId { get; }
    public DateTime StartDateTime { get; }
    public DateTime EndDateTime { get; }
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return AppointmentId;
        yield return StartDateTime;
        yield return EndDateTime;
    }
    public bool OverlapsDateTime(PatientAppointment other)
    {
        return (StartDateTime.Date == other.StartDateTime.Date
                && StartDateTime < other.StartDateTime 
                && EndDateTime > other.StartDateTime) 
               ||
               (EndDateTime.Date == other.EndDateTime.Date
                && StartDateTime < other.EndDateTime 
                && EndDateTime > other.EndDateTime);
    }

    public static PatientAppointment Create(DateTime appointmentDateTime, int durationMinutes, Guid appointmentId)
    {
        return new PatientAppointment(appointmentId,
            appointmentDateTime,
            appointmentDateTime.AddMinutes(durationMinutes));
    }
}
public class Patient : Entity
{
    private const int MaxDailyAppointments = 2;
    private readonly List<PatientAppointment> _appointments = new();
    private Dictionary<DateOnly, List<PatientAppointment>> _appointmentsByDate =>
        _appointments.GroupBy(a => a.StartDateTime.Date)
            .ToDictionary(g => new DateOnly(g.Key.Year, g.Key.Month, g.Key.Day), g => g.ToList());

    public Patient(Guid? id = null) : base(id ?? Guid.NewGuid())
    {

    }

    public static ErrorOr<Patient> Create(Guid? id = null)
    {
        var patient = new Patient(id ?? Guid.NewGuid());
        return patient;
    }

    public ErrorOr<Success> AddAppointment(Appointment appointment)
    {
        var patientAppointment = PatientAppointment.Create(appointment.AppointmentDate, appointment.AppointmentDurationMinutes, appointment.Id);
        _appointmentsByDate.TryGetValue(new DateOnly(appointment.AppointmentDate.Year, appointment.AppointmentDate.Month, appointment.AppointmentDate.Day), out var appointmentsByDate);

        if (appointmentsByDate == null)
        {
            appointmentsByDate = new List<PatientAppointment>();
            _appointmentsByDate.Add(new DateOnly(appointment.AppointmentDate.Year, appointment.AppointmentDate.Month, appointment.AppointmentDate.Day), appointmentsByDate);
        }


        if (appointmentsByDate.Any(a => a.OverlapsDateTime(patientAppointment)))
        {
            return Error.Conflict(description: "Patient already has an appointment at that time",
                code: PatientErrors.AppointmentOverlap);
        }
        if (appointmentsByDate.Count >= MaxDailyAppointments)
        {
            return Error.Conflict(description: $"Patient cannot have more than {MaxDailyAppointments} appointments per day",
                code: PatientErrors.AppointmentCountExceeded);
        }


        _appointments.Add(patientAppointment);
        return Result.Success;
    }
}