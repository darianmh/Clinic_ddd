using Clinic.Domain;
using ErrorOr;

namespace Clinic.Application.Contracts.Repositories;
public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default);
}