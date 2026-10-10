using ClinicBooking.Models;
namespace ClinicBooking.ViewModels;

public class BookViewModel
{
    public Doctor Doctor { get; set; } = null!;
    public DateTime Date { get; set; }
    public List<DateTime> Slots { get; set; } = new();
}

public class AdminDashboardViewModel
{
    public int TotalAppointments { get; set; }
    public int TodayAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public int DoctorCount { get; set; }
    public List<Appointment> Appointments { get; set; } = new();
}
