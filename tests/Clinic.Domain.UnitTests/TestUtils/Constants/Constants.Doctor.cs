namespace Clinic.Domain.UnitTests.TestUtils.TestConstants;

public static partial class Constants
{
    public static class Doctor
    {
        public static Guid GeneralDoctorId = Guid.NewGuid();
        public static Guid SpecialistDoctorId = Guid.NewGuid();
        public static Domain.Doctor GeneralDoctor =
        DoctorFactory.CreateDoctor(
            DoctorType.General,
            GeneralDoctorId
        );
        public static Domain.Doctor SpecialistDoctor =
         DoctorFactory.CreateDoctor(
            DoctorType.Specialist,
            SpecialistDoctorId
        );

    }
}
