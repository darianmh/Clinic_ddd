using Clinic.Domain;
using ErrorOr;

namespace Clinic.Application.Contracts.Repositories;
public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
}
