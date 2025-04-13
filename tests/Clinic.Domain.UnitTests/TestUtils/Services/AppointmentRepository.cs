using Clinic.Application.Contracts.Repositories;

namespace Clinic.Domain.UnitTests.TestUtils.Services;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly List<Appointment> _appointments = new List<Appointment>();

    public async Task<Appointment?> GetByIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == appointmentId);
        return appointment;
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        _appointments.Add(appointment);
    }
}