using ErrorOr;
using MediatR;

namespace Clinic.Application.Appointments.Command.CreateAppointment;
public record CreateAppointmentCommand : IRequest<ErrorOr<Domain.Appointment>>
{
    public CreateAppointmentCommand(
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