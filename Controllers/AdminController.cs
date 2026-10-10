using ClinicBooking.Data;
using ClinicBooking.Models;
using ClinicBooking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ClinicBooking.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    public AdminController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var vm = new AdminDashboardViewModel
        {
            TotalAppointments = await _db.Appointments.CountAsync(),
            TodayAppointments = await _db.Appointments.CountAsync(a => a.StartTime >= today && a.StartTime < today.AddDays(1) && a.Status != AppointmentStatus.Cancelled),
            PendingAppointments = await _db.Appointments.CountAsync(a => a.Status == AppointmentStatus.Pending),
            DoctorCount = await _db.Doctors.CountAsync(),
            Appointments = await _db.Appointments
                .Include(a => a.Doctor).Include(a => a.Patient)
                .OrderBy(a => a.StartTime).Where(a => a.StartTime >= today).Take(50).ToListAsync()
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, AppointmentStatus status)
    {
        var appt = await _db.Appointments.FindAsync(id);
        if (appt is null) return NotFound();
        appt.Status = status;
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Appointment #{id} marked {status}.";
        return RedirectToAction(nameof(Index));
    }
}
