using Clinic.Domain.Common;
using ErrorOr;

namespace Clinic.Domain;
public class Doctor : Entity
{

    public Doctor()
    {

    }
    public Doctor(
        DoctorType? doctorType,
        Guid? id) : base(id ?? Guid.NewGuid())
    {
        _doctorType = doctorType ?? DoctorType.General;
    }

    private readonly DoctorType _doctorType;
    private readonly List<Guid> _appointments = new List<Guid>();


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

    public TimeDuration GetValidAppointmentDuration()
    {
        switch (_doctorType)
        {
            case DoctorType.General:
                return TimeDuration.Create(5, 15);
            case DoctorType.Specialist:
                return TimeDuration.Create(10, 30);
            default:
                throw new ArgumentOutOfRangeException(nameof(_doctorType), _doctorType, null);
        }
    }
}