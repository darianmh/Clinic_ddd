using Clinic.Application.Contracts.Repositories;
using Clinic.Domain.PatientAggregae;

namespace Clinic.Domain.UnitTests.TestUtils.Services;

public class PatientRepository : IPatientRepository
{
    private readonly List<Patient> _patients = new();
    public Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_patients.FirstOrDefault(p => p.Id == id));
    }
}