using AllStay.Domain.Entities;
using AllStay.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Persistence;

/// <summary>Seeds one demo hotel end-to-end so all three surfaces (guest PWA, backoffice, institutional site) can be demoed together.</summary>
public static class DbSeeder
{
    public const string DemoHotelCode = "curacau-resort";
    public const string DemoStaffEmail = "manager@curacauresort.com";
    public const string DemoStaffPassword = "AllStay@2026";

    public static async Task SeedAsync(AllStayDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (await db.Hotels.AnyAsync(ct)) return;

        var hotel = new Hotel
        {
            Name = "Curaçau Resort & Spa",
            Code = DemoHotelCode,
            Tier = HotelTier.Professional,
            ContactEmail = "contato@curacauresort.com",
            Address = "Av. Beira Mar, 1200",
            City = "Fortaleza, CE"
        };
        db.Hotels.Add(hotel);

        db.HotelStaffUsers.Add(new HotelStaffUser
        {
            HotelId = hotel.Id,
            Hotel = hotel,
            Email = DemoStaffEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoStaffPassword),
            FullName = "Ana Souza",
            Role = StaffRole.Manager
        });

        var now = DateTimeOffset.UtcNow.Date;

        var activities = new[]
        {
            new Activity
            {
                HotelId = hotel.Id, Hotel = hotel, Name = "Aula de Yoga ao Nascer do Sol",
                Description = "Sessão de yoga na praia com instrutor certificado.",
                Category = "Bem-estar", Price = 0m, DurationMinutes = 60
            },
            new Activity
            {
                HotelId = hotel.Id, Hotel = hotel, Name = "Spa - Massagem Relaxante",
                Description = "Massagem terapêutica de 50 minutos.",
                Category = "Spa", Price = 350m, DurationMinutes = 50
            },
            new Activity
            {
                HotelId = hotel.Id, Hotel = hotel, Name = "Passeio de Catamarã ao Pôr do Sol",
                Description = "Passeio de barco com open bar e vista panorâmica.",
                Category = "Passeios", Price = 480m, DurationMinutes = 120
            },
            new Activity
            {
                HotelId = hotel.Id, Hotel = hotel, Name = "Jantar Romântico à Beira-Mar",
                Description = "Mesa exclusiva na areia com menu degustação.",
                Category = "Gastronomia", Price = 620m, DurationMinutes = 90
            }
        };

        db.Activities.AddRange(activities);

        foreach (var activity in activities)
        {
            for (var day = 0; day < 3; day++)
            {
                activity.Slots.Add(new ActivitySlot
                {
                    Activity = activity,
                    StartTime = now.AddDays(day).AddHours(9 + day * 3),
                    Capacity = 8
                });
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
