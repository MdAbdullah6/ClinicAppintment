using ClinicBooking.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ClinicBooking.Controllers;

public class DoctorsController : Controller
{
    private readonly ApplicationDbContext _db;
    public DoctorsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(int? specialtyId)
    {
        ViewBag.Specialties = await _db.Specialties.OrderBy(s => s.Name).ToListAsync();
        ViewBag.SelectedSpecialty = specialtyId;
        var q = _db.Doctors.Include(d => d.Specialty).AsQueryable();
        if (specialtyId.HasValue) q = q.Where(d => d.SpecialtyId == specialtyId);
        return View(await q.OrderBy(d => d.FullName).ToListAsync());
    }
}
