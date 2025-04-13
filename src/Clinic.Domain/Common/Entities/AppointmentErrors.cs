namespace Clinic.Domain;

public static class AppointmentErrors
{
    public const string InvalidAppointmentDate = "Appointment.InvalidAppointmentDate";
    public const string InvalidAppointmentDuration = "Appointment.InvalidAppointmentDuration";
    public const string AppointmentMaxOverlapExceeded = "Appointment.AppointmentMaxOverlapExceeded";
}
