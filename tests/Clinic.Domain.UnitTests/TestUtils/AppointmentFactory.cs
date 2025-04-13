using Clinic.Domain.UnitTests.TestUtils.TestConstants;
using ErrorOr;

namespace Clinic.Domain.UnitTests.TestUtils;

public static class AppointmentFactory
{
    public static ErrorOr<Appointment> CreateAppointment(
        DateTime? appointmentDate = null,
        uint? appointmentDurationMinutes = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? id = null)
    {
        return Appointment.Create(
            appointmentDate ?? Constants.Date.ValidAppointmentDateTime,
            appointmentDurationMinutes ?? 15,
            doctorId ?? Constants.Doctor.GeneralDoctorId,
            patientId?? Constants.Patient.PatientId,
            id ?? Guid.NewGuid());
    }
}