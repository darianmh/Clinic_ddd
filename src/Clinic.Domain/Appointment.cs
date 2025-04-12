using Clinic.Domain.Common;

namespace Clinic.Domain;

public class Appointment : Entity
{
    public Appointment(Guid? id) : base(id ?? Guid.NewGuid())
    {
    }
/*************  ✨ Windsurf Command ⭐  *************/
/// <summary>
/// Initializes a new instance of the <see cref="Appointment"/> class with a new unique identifier.
/// </summary>

/*******  d4298ba7-be3f-4ac0-aaf9-bd67da2c3274  *******/
    public Appointment()
    {

    }
}
