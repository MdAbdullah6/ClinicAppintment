using ClinicBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace ClinicBooking.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();

        var roles = sp.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roles.RoleExistsAsync("Admin"))
            await roles.CreateAsync(new IdentityRole("Admin"));

        var users = sp.GetRequiredService<UserManager<ApplicationUser>>();
        const string adminEmail = "admin@clinic.com";
        if (await users.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, FullName = "Clinic Admin" };
            await users.CreateAsync(admin, "Admin@123");
            await users.AddToRoleAsync(admin, "Admin");
        }

        if (!await db.Specialties.AnyAsync())
        {
            var gp = new Specialty { Name = "General Medicine" };
            var cardio = new Specialty { Name = "Cardiology" };
            var derm = new Specialty { Name = "Dermatology" };
            var peds = new Specialty { Name = "Pediatrics" };
            db.Specialties.AddRange(gp, cardio, derm, peds);
            db.Doctors.AddRange(
                new Doctor { FullName = "Dr. Ayesha Rahman", Specialty = gp, ConsultationFee = 500, Bio = "10 years of experience in family medicine." },
                new Doctor { FullName = "Dr. Imran Hossain", Specialty = cardio, ConsultationFee = 1200, Bio = "Heart specialist focused on preventive care.", SlotMinutes = 45 },
                new Doctor { FullName = "Dr. Nusrat Jahan", Specialty = derm, ConsultationFee = 800, Bio = "Skin, hair and allergy treatment.", WorkStart = new TimeSpan(10, 0, 0), WorkEnd = new TimeSpan(16, 0, 0) },
                new Doctor { FullName = "Dr. Tanvir Ahmed", Specialty = peds, ConsultationFee = 700, Bio = "Child health and vaccinations.", SlotMinutes = 20 });
            await db.SaveChangesAsync();
        }
    }
}
