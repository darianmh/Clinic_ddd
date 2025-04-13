using ErrorOr;
using MediatR;

namespace Clinic.Application.Appointments.Command.CreateAppointment;
public record CreateAppointmentCommand : IRequest<ErrorOr<Domain.Appointment>>
{
    public CreateAppointmentCommand(
        DateTime appointmentDate,
        uint appointmentDurationMinutes,
        Guid doctorId, 
        Guid patientId)
    {
        AppointmentDate = appointmentDate;
        AppointmentDurationMinutes = appointmentDurationMinutes;
        DoctorId = doctorId;
        PatientId = patientId;
    }

    public DateTime AppointmentDate { get; }
    public uint AppointmentDurationMinutes { get; }
    public Guid DoctorId { get; }
    public Guid PatientId { get; }
}