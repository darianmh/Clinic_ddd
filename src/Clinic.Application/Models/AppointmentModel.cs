using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Application.Models;

public readonly struct AppointmentModel(
    DateTime startDateTime,
    DateTime endDateTime)
{
    public DateTime StartDateTime { get; } = startDateTime;
    public DateTime EndDateTime { get; } = endDateTime;


    public static explicit operator AppointmentModel(Domain.Appointment entity)
    {
        return new AppointmentModel(entity.StartDateTime, entity.EndDateTime);
    }
}