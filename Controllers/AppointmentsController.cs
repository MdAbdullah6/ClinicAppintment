using System.Security.Claims;
using ClinicBooking.Data;
using ClinicBooking.Services;
using ClinicBooking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ClinicBooking.Controllers;

[Authorize]
public class AppointmentsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IAppointmentService _service;
    public AppointmentsController(ApplicationDbContext db, IAppointmentService service)
    { _db = db; _service = service; }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> Book(int doctorId, DateTime? date)
    {
        var doctor = await _db.Doctors.Include(d => d.Specialty).FirstOrDefaultAsync(d => d.Id == doctorId);
        if (doctor is null) return NotFound();

        var day = (date ?? DateTime.Today).Date;
        if (day < DateTime.Today) day = DateTime.Today;

        return View(new BookViewModel
        {
            Doctor = doctor,
            Date = day,
            Slots = await _service.GetAvailableSlotsAsync(doctorId, day)
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int doctorId, DateTime start, string? reason)
    {
        var (ok, error) = await _service.BookAsync(doctorId, UserId, start, reason);
        if (!ok)
        {
            TempData["Error"] = error;
            return RedirectToAction(nameof(Book), new { doctorId, date = start.ToString("yyyy-MM-dd") });
        }
        TempData["Success"] = "Appointment booked! The clinic will confirm it shortly.";
        return RedirectToAction(nameof(My));
    }

    public async Task<IActionResult> My()
    {
        var list = await _db.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d!.Specialty)
            .Where(a => a.PatientId == UserId)
            .OrderByDescending(a => a.StartTime)
            .ToListAsync();
        return View(list);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var (ok, error) = await _service.CancelAsync(id, UserId, User.IsInRole("Admin"));
        TempData[ok ? "Success" : "Error"] = ok ? "Appointment cancelled." : error;
        return RedirectToAction(nameof(My));
    }
}
