using Clinic.Domain.PatientAggregae;
using ErrorOr;

namespace Clinic.Domain.UnitTests.TestUtils;

public static class PatientFactory
{
    public static ErrorOr<Patient> CreatePatient(Guid? id = null)
    {
        return Patient.Create(id);
    }
}