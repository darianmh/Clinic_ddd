using Clinic.Application.Contracts.Repositories;

namespace Clinic.Domain.UnitTests.TestUtils.Services;

public class DoctorRepository : IDoctorRepository{
    private readonly List<Doctor> _doctors = new List<Doctor>();

    public Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        _doctors.Add(doctor);
        return Task.CompletedTask;
    }

    public async Task<Doctor?> GetByIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        var doctor = _doctors.FirstOrDefault(d => d.Id == doctorId);
        return doctor;
    }
    
}
