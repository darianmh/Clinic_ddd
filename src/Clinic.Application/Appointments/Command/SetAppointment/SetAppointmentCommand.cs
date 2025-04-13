using ErrorOr;
using MediatR;

namespace Clinic.Application.Appointments.Command.SetAppointment;
public record SetAppointmentCommand : IRequest<ErrorOr<Domain.Appointment>>
{
    public SetAppointmentCommand(
        DateTime startDateTime,
        uint durationMinutes,
        Guid doctorId, 
        Guid patientId)
    {
        StartDateTime = startDateTime;
        DurationMinutes = durationMinutes;
        DoctorId = doctorId;
        PatientId = patientId;
    }

    public DateTime StartDateTime { get; }
    public uint DurationMinutes { get; }
    public Guid DoctorId { get; }
    public Guid PatientId { get; }
}