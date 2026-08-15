using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class HotelService(AllStayDbContext db) : IHotelService
{
    public async Task<HotelDto?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var hotel = await db.Hotels.FirstOrDefaultAsync(h => h.Code == code && h.IsActive, ct);
        return hotel is null ? null : ToDto(hotel);
    }

    public async Task<HotelDto> CreateAsync(CreateHotelRequest request, CancellationToken ct = default)
    {
        var hotel = new Hotel
        {
            Name = request.Name,
            Code = request.Code,
            Tier = request.Tier,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone
        };

        db.Hotels.Add(hotel);
        await db.SaveChangesAsync(ct);
        return ToDto(hotel);
    }

    public async Task<IReadOnlyList<HotelDto>> ListAsync(CancellationToken ct = default)
    {
        return await db.Hotels
            .OrderBy(h => h.Name)
            .Select(h => new HotelDto(h.Id, h.Name, h.Code, h.Tier, h.IsActive))
            .ToListAsync(ct);
    }

    public async Task<StaffProfileDto?> CreateStaffAsync(Guid hotelId, CreateStaffRequest request, CancellationToken ct = default)
    {
        var hotel = await db.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId, ct);
        if (hotel is null) return null;

        var staff = new HotelStaffUser
        {
            HotelId = hotelId,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            Role = request.Role
        };

        db.HotelStaffUsers.Add(staff);
        await db.SaveChangesAsync(ct);

        return new StaffProfileDto(staff.Id, staff.Email, staff.FullName, staff.Role, hotel.Id, hotel.Name);
    }

    private static HotelDto ToDto(Hotel h) => new(h.Id, h.Name, h.Code, h.Tier, h.IsActive);
}
