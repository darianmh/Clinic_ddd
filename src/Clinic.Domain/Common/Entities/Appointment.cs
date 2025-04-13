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
        var validateAppointmentDateResult = WorkingHours.ValidateWorkingDate(startDateTime);
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

}
