using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class GuestRequestService(AllStayDbContext db) : IGuestRequestService
{
    public async Task<IReadOnlyList<GuestRequestDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var requests = await db.GuestRequests
            .Where(r => r.HotelId == hotelId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return requests.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<GuestRequestDto>> ListForGuestAsync(Guid hotelId, string roomNumber, CancellationToken ct = default)
    {
        var requests = await db.GuestRequests
            .Where(r => r.HotelId == hotelId && r.RoomNumber == roomNumber)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return requests.Select(ToDto).ToList();
    }

    public async Task<GuestRequestDto> CreateAsync(Guid hotelId, CreateGuestRequestRequest request, CancellationToken ct = default)
    {
        var guestRequest = new GuestRequest
        {
            HotelId = hotelId,
            Type = request.Type,
            Details = request.Details,
            GuestName = request.GuestName,
            RoomNumber = request.RoomNumber
        };

        db.GuestRequests.Add(guestRequest);
        await db.SaveChangesAsync(ct);
        return ToDto(guestRequest);
    }

    public async Task<GuestRequestDto?> UpdateStatusAsync(Guid hotelId, Guid requestId, UpdateGuestRequestStatusRequest request, CancellationToken ct = default)
    {
        var guestRequest = await db.GuestRequests.FirstOrDefaultAsync(r => r.HotelId == hotelId && r.Id == requestId, ct);
        if (guestRequest is null) return null;

        guestRequest.Status = request.Status;
        await db.SaveChangesAsync(ct);
        return ToDto(guestRequest);
    }

    private static GuestRequestDto ToDto(GuestRequest r) => new(
        r.Id,
        r.HotelId,
        r.Type,
        r.Details,
        r.GuestName,
        r.RoomNumber,
        r.Status,
        r.CreatedAt);
}
