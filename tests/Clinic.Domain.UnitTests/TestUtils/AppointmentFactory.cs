namespace Clinic.Domain.UnitTests.TestUtils;

public static class AppointmentFactory
{
    public static Appointment CreateAppointment(
        DateTime? appointmentDate = null,
        Guid? id = null)
    {
        return new Appointment(
            appointmentDate ?? DateTime.Now.AddDays(1),
            id ?? Guid.NewGuid());
    }
}