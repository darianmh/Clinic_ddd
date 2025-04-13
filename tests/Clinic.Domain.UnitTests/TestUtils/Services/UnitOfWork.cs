using Clinic.Application.Contracts.Repositories;

namespace Clinic.Domain.UnitTests.TestUtils.Services;

public class UnitOfWork : IUnitOfWork
{
    public IDoctorRepository DoctorRepository { get; } = new DoctorRepository();
    public IPatientRepository PatientRepository { get; }= new PatientRepository();
    public IAppointmentRepository AppointmentRepository { get; }= new AppointmentRepository();
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(1);
    }
}