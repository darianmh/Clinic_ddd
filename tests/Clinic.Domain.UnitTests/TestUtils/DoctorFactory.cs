namespace Clinic.Domain.UnitTests.TestUtils;

public static class DoctorFactory
{
    public static Doctor CreateDoctor(Guid? id = null)
    {
        return new Doctor(id ?? Guid.NewGuid());
    }
}