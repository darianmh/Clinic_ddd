namespace Clinic.Application.Contracts.Repositories;

public interface IUnitOfWork
{
    IDoctorRepository DoctorRepository { get; }
    IPatientRepository PatientRepository { get; }
    IAppointmentRepository AppointmentRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}