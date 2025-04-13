using Clinic.Application.Contracts.Repositories;
using Clinic.Domain;
using ErrorOr;
using MediatR;

namespace Clinic.Application.Appointments.Command.CreateAppointment;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, ErrorOr<Appointment>>
{

    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<ErrorOr<Appointment>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor == null)
            return Error.NotFound(
                description: "Doctor not found.");



        var result = Appointment.Create(request.AppointmentDate, request.AppointmentDurationMinutes, doctor.Id);
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




    public CreateAppointmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _appointmentRepository = unitOfWork.AppointmentRepository;
        _doctorRepository = unitOfWork.DoctorRepository;
        _unitOfWork = unitOfWork;
    }
}
