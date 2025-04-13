using Clinic.Application.Models;
using ErrorOr;
using MediatR;

namespace Clinic.Application.Appointments.Command.SetEarliestAppointment;

public record SetEarliestAppointmentCommand(
    Guid DoctorId,
    Guid PatientId,
    uint DurationMinutes)
    : IRequest<ErrorOr<AppointmentModel>>
    ;