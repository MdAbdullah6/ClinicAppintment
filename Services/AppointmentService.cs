using ClinicBooking.Data;
using ClinicBooking.Models;
using Microsoft.EntityFrameworkCore;
namespace ClinicBooking.Services;

public interface IAppointmentService
{
    Task<List<DateTime>> GetAvailableSlotsAsync(int doctorId, DateTime date);
    Task<(bool Ok, string? Error)> BookAsync(int doctorId, string patientId, DateTime start, string? reason);
    Task<(bool Ok, string? Error)> CancelAsync(int appointmentId, string userId, bool isAdmin);
}

public class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _db;
    public AppointmentService(ApplicationDbContext db) => _db = db;

    public async Task<List<DateTime>> GetAvailableSlotsAsync(int doctorId, DateTime date)
    {
        var doctor = await _db.Doctors.FindAsync(doctorId);
        if (doctor is null) return new();

        var day = date.Date;
        var next = day.AddDays(1);
        var booked = await _db.Appointments
            .Where(a => a.DoctorId == doctorId && a.Status != AppointmentStatus.Cancelled
                        && a.StartTime >= day && a.StartTime < next)
            .Select(a => new { a.StartTime, a.EndTime })
            .ToListAsync();

        var slots = new List<DateTime>();
        var step = TimeSpan.FromMinutes(doctor.SlotMinutes);
        var cur = day + doctor.WorkStart;
        var end = day + doctor.WorkEnd;
        while (cur + step <= end)
        {
            var slotEnd = cur + step;
            var taken = booked.Any(b => cur < b.EndTime && slotEnd > b.StartTime);
            if (!taken && cur > DateTime.Now) slots.Add(cur);
            cur = slotEnd;
        }
        return slots;
    }

    public async Task<(bool Ok, string? Error)> BookAsync(int doctorId, string patientId, DateTime start, string? reason)
    {
        var doctor = await _db.Doctors.FindAsync(doctorId);
        if (doctor is null) return (false, "Doctor not found.");

        var available = await GetAvailableSlotsAsync(doctorId, start);
        if (!available.Contains(start))
            return (false, "That time slot is no longer available. Please pick another.");

        var end = start.AddMinutes(doctor.SlotMinutes);
        var patientBusy = await _db.Appointments.AnyAsync(a =>
            a.PatientId == patientId && a.Status != AppointmentStatus.Cancelled
            && start < a.EndTime && end > a.StartTime);
        if (patientBusy) return (false, "You already have an appointment at that time.");

        _db.Appointments.Add(new Appointment
        {
            DoctorId = doctorId, PatientId = patientId,
            StartTime = start, EndTime = end, Reason = reason
        });
        await _db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Ok, string? Error)> CancelAsync(int appointmentId, string userId, bool isAdmin)
    {
        var appt = await _db.Appointments.FindAsync(appointmentId);
        if (appt is null) return (false, "Appointment not found.");
        if (!isAdmin && appt.PatientId != userId) return (false, "You cannot cancel this appointment.");
        if (appt.Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            return (false, "This appointment can no longer be cancelled.");

        appt.Status = AppointmentStatus.Cancelled;
        await _db.SaveChangesAsync();
        return (true, null);
    }
}
