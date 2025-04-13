using Clinic.Application.Contracts.Repositories;
using Clinic.Domain;
using ErrorOr;
using MediatR;

namespace Clinic.Application.Appointments.Command.CreateAppointment;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, ErrorOr<Appointment>>
{

    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;

    public async Task<ErrorOr<Appointment>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId);
        if (doctor == null)
            return Error.NotFound(
                description: "Doctor not found.");

        var validateAppointmentDuration = ValidateAppointmentDuration(doctor, request.AppointmentDurationMinutes);
        if (validateAppointmentDuration.IsError)
            return validateAppointmentDuration.Errors;

        var result = Appointment.Create(request.AppointmentDate, request.AppointmentDurationMinutes, doctor.Id);
        if (result.IsError)
            return result.Errors;

        await _appointmentRepository.AddAsync(result.Value);

        return result.Value;
    }


    private static ErrorOr<Success> ValidateAppointmentDuration(Doctor doctor, int appointmentDurationMinutes)
    {
        var validAppointmentDuration = doctor.GetValidAppointmentDuration();
        if (appointmentDurationMinutes < validAppointmentDuration.MinTime ||
            appointmentDurationMinutes > validAppointmentDuration.MaxTime)
        {
            return Error.Validation(
                description: $"Appointment duration must be between {validAppointmentDuration.MinTime} and {validAppointmentDuration.MaxTime} minutes.",
                code: AppointmentError.InvalidAppointmentDuration);
        }
        return Result.Success;
    }
    
    public CreateAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IDoctorRepository doctorRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
    }
}
