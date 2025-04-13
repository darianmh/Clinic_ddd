using ErrorOr;
using MediatR;

namespace Clinic.Application.Appointments.Command.CreateAppointment;
public record CreateAppointmentCommand : IRequest<ErrorOr<Domain.Appointment>>
{
    public CreateAppointmentCommand(
        DateTime appointmentDate,
        int appointmentDurationMinutes,
        Guid doctorId)
    {
        AppointmentDate = appointmentDate;
        AppointmentDurationMinutes = appointmentDurationMinutes;
        DoctorId = doctorId;
    }

    public DateTime AppointmentDate { get; }
    public int AppointmentDurationMinutes { get; }
    public Guid DoctorId { get; }
}