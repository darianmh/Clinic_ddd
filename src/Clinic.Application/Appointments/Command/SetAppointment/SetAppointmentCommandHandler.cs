using Clinic.Application.Contracts.Repositories;
using Clinic.Domain;
using ErrorOr;
using MediatR;

namespace Clinic.Application.Appointments.Command.SetAppointment;

public class SetAppointmentCommandHandler : IRequestHandler<SetAppointmentCommand, ErrorOr<Appointment>>
{

    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<ErrorOr<Appointment>> Handle(SetAppointmentCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor == null)
            return Error.NotFound(
                description: "Doctor not found.");

        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient == null)
            return Error.NotFound(
                description: "Patient not found.");


        var result = Appointment.Create(request.StartDateTime,
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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value;
    }




    public SetAppointmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _appointmentRepository = unitOfWork.AppointmentRepository;
        _doctorRepository = unitOfWork.DoctorRepository;
        _patientRepository = unitOfWork.PatientRepository;
        _unitOfWork = unitOfWork;
    }
}
