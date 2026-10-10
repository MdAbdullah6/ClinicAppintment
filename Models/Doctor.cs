using System.ComponentModel.DataAnnotations;
namespace ClinicBooking.Models;

public class Doctor
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string FullName { get; set; } = "";
    public int SpecialtyId { get; set; }
    public Specialty? Specialty { get; set; }
    [StringLength(500)] public string? Bio { get; set; }
    public decimal ConsultationFee { get; set; }
    public TimeSpan WorkStart { get; set; } = new(9, 0, 0);
    public TimeSpan WorkEnd { get; set; } = new(17, 0, 0);
    public int SlotMinutes { get; set; } = 30;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
