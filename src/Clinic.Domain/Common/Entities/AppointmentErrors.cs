namespace Clinic.Domain;

public static class AppointmentErrors
{
    public const string InvalidDurationMinutes = "Appointment.InvalidDurationMinutes";
    public const string InvalidAppointmentDuration = "Appointment.InvalidAppointmentDuration";
    public const string AppointmentMaxOverlapExceeded = "Appointment.AppointmentMaxOverlapExceeded";
}
