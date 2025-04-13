using Clinic.Domain.UnitTests.TestUtils.TestConstants;
using ErrorOr;

namespace Clinic.Domain.UnitTests.TestUtils;

public static class AppointmentFactory
{
    public static ErrorOr<Appointment> CreateAppointment(
        DateTime? appointmentDate = null,
        int? appointmentDurationMinutes = null,
        Guid? doctorId = null,
        Guid? id = null)
    {
        return Appointment.Create(
            appointmentDate ?? Constants.Date.ValidAppointmentDateTime,
            appointmentDurationMinutes ?? 15,
            doctorId ?? Constants.Doctor.GeneralDoctorId,
            id ?? Guid.NewGuid());
    }
}