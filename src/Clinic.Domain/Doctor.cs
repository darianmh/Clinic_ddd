using Clinic.Domain.Common;
using ErrorOr;

namespace Clinic.Domain;
public class Doctor : Entity
{

    public Doctor()
    {

    }
    public Doctor(Guid? id) : base(id ?? Guid.NewGuid())
    {

    }

    private readonly List<Guid> _appointments=new List<Guid>();


    public ErrorOr<Success> AddAppointment(Guid appointmentId)
    {
        if (_appointments.Contains(appointmentId))
        {
            return Error.Validation(
                description: "The appointment already exists."
            );
        }
        _appointments.Add(appointmentId);
        return Result.Success;
    }
}