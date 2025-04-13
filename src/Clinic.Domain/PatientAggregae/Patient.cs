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
        return (StartDateTime.Date == other.EndDateTime.Date && StartDateTime < other.EndDateTime)
            || (EndDateTime.Date == other.StartDateTime.Date && EndDateTime > other.StartDateTime);
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
    private readonly List<PatientAppointment> _appointments = new();

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
        if (_appointments.Any(a => a.OverlapsDateTime(patientAppointment)))
        {
            return Error.Conflict(description: "Patient already has an appointment at that time",
                code: PatientErrors.AppointmentOverlap);
        }

        _appointments.Add(patientAppointment);
        return Result.Success;
    }
}