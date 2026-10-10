using ClinicBooking.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace ClinicBooking.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<Appointment>().HasIndex(a => new { a.DoctorId, a.StartTime });
        b.Entity<Appointment>().Property(a => a.Status).HasConversion<string>();
        b.Entity<Doctor>().Property(d => d.ConsultationFee).HasPrecision(10, 2);
    }
}
