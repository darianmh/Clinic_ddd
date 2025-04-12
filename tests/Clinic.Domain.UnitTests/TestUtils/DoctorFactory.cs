using Clinic.Domain;

namespace Clinic.Domain.UnitTests.TestUtils;

public static class DoctorFactory
{
    public static Doctor CreateDoctor(Guid? id = null)
    {
        return new Doctor(id ?? Guid.NewGuid());
    }
}

public static class AppointmentFactory
{
    public static Appointment CreateAppointment(Guid? id = null)
    {
        return new Appointment(id ?? Guid.NewGuid());
    }
}