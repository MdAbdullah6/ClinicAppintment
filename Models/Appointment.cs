using System.ComponentModel.DataAnnotations;
namespace ClinicBooking.Models;

public enum AppointmentStatus { Pending, Confirmed, Completed, Cancelled }

public class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public string PatientId { get; set; } = "";
    public ApplicationUser? Patient { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    [StringLength(300)] public string? Reason { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
