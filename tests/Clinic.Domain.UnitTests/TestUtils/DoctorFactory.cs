namespace Clinic.Domain.UnitTests.TestUtils;

public static class DoctorFactory
{
    public static Doctor CreateDoctor(
        DoctorType? doctorType = null,
        Guid? id = null)
    {
        return Doctor.Create(doctorType?? DoctorType.General, id ?? Guid.NewGuid());
    }
}