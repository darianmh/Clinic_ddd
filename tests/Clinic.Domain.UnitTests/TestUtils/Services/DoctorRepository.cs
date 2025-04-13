using Clinic.Application.Contracts.Repositories;

namespace Clinic.Domain.UnitTests.TestUtils.Services;

public class DoctorRepository : IDoctorRepository
{
    private readonly List<Doctor> _doctors = new List<Doctor>();

    public Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        _doctors.Add(doctor);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        var index = _doctors.FindIndex(d => d.Id == doctor.Id);
        _doctors[index] = doctor;
        return Task.CompletedTask;
    }

    public async Task<Doctor?> GetByIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        var doctor = _doctors.FirstOrDefault(d => d.Id == doctorId);
        return doctor;
    }

}
