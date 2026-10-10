# MediBook - Clinic Appointment Booking System

ASP.NET Core 8 MVC + Identity + EF Core (SQLite).

## Features
- Patient registration/login (ASP.NET Core Identity), role-based admin area
- Browse doctors by specialty
- Slot generation per doctor (working hours + slot length) with conflict prevention
- Patients cannot double-book themselves; bookings validated server-side
- Patient: book, view, cancel appointments
- Admin: dashboard stats, confirm / complete / cancel appointments

## Run
```bash
dotnet restore
dotnet run
```
Open the URL shown in the console. Seeded admin: `admin@clinic.com` / `Admin@123`.
The SQLite database (`clinic.db`) is created and seeded automatically.

## Structure
- `Models/` entities - `Data/` DbContext + seeding - `Services/` booking business logic
- `Controllers/` thin controllers - `Views/` Razor + Bootstrap 5

## Roadmap
Email notifications, doctor accounts/schedules, xUnit tests for AppointmentService,
EF migrations + PostgreSQL/SQL Server, Docker, deploy to Azure.
