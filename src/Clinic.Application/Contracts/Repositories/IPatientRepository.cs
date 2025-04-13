using Clinic.Domain.PatientAggregae;

namespace Clinic.Application.Contracts.Repositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}