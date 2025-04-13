using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Domain.DoctorAggregate
{
    public static class DoctorErrors
    {
        public const string InvalidSchedule = "Doctor.InvalidSchedule";
        public const string DuplicatedSchedule = "Doctor.DuplicatedSchedule";
    }
}
