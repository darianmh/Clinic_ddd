using Clinic.Application.Contracts.Repositories;
using Clinic.Application.Models;
using Clinic.Domain;
using ErrorOr;
using MediatR;

namespace Clinic.Application.Appointments.Command.SetEarliestAppointment;

public class SetEarliestAppointmentCommandHandler
    (IUnitOfWork unitOfWork)
    : IRequestHandler<SetEarliestAppointmentCommand, ErrorOr<AppointmentModel>>
{
    private readonly IAppointmentRepository _appointmentRepository = unitOfWork.AppointmentRepository;
    private readonly IDoctorRepository _doctorRepository = unitOfWork.DoctorRepository;
    private readonly IPatientRepository _patientRepository = unitOfWork.PatientRepository;
    public async Task<ErrorOr<AppointmentModel>> Handle(SetEarliestAppointmentCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor == null)
            return Error.NotFound(
                description: "Doctor not found.");

        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient == null)
            return Error.NotFound(
                description: "Patient not found.");

        var earliestAvailableDateTime = doctor.FindEarliestAvailableAppointment(request.DurationMinutes);
        if (earliestAvailableDateTime == null)
            return Error.Conflict(
                description: "Did not found any available time.");

        var result = Appointment.Create(earliestAvailableDateTime.Value,
            request.DurationMinutes,
            doctor.Id,
            patient.Id
        );
        if (result.IsError)
            return result.Errors;

        var addAppointmentResult = doctor.AddAppointment(result.Value);
        if (addAppointmentResult.IsError)
            return addAppointmentResult.Errors;

        await _appointmentRepository.AddAsync(result.Value, cancellationToken);
        await _doctorRepository.UpdateAsync(doctor, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return (AppointmentModel)result.Value;
    }
}