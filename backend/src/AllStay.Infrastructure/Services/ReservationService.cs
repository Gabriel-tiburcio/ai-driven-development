using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Domain.Enums;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class ReservationService(AllStayDbContext db) : IReservationService
{
    public async Task<IReadOnlyList<ReservationDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var reservations = await db.Reservations
            .Include(r => r.Slot!).ThenInclude(s => s.Activity)
            .Where(r => r.Slot!.Activity!.HotelId == hotelId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return reservations.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<ReservationDto>> ListForGuestAsync(Guid hotelId, string roomNumber, CancellationToken ct = default)
    {
        var reservations = await db.Reservations
            .Include(r => r.Slot!).ThenInclude(s => s.Activity)
            .Where(r => r.Slot!.Activity!.HotelId == hotelId && r.RoomNumber == roomNumber && r.Status == ReservationStatus.Confirmed)
            .OrderBy(r => r.Slot!.StartTime)
            .ToListAsync(ct);

        return reservations.Select(ToDto).ToList();
    }

    public async Task<ReservationDto?> CreateAsync(Guid hotelId, CreateReservationRequest request, CancellationToken ct = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var slot = await db.ActivitySlots
            .Include(s => s.Activity)
            .FirstOrDefaultAsync(s => s.Id == request.SlotId && s.Activity!.HotelId == hotelId, ct);

        if (slot is null) return null;

        if (slot.AvailableSpots <= 0)
            throw new InvalidOperationException("This time slot is fully booked.");

        var reservation = new Reservation
        {
            SlotId = slot.Id,
            GuestName = request.GuestName,
            RoomNumber = request.RoomNumber
        };

        slot.BookedCount += 1;
        db.Reservations.Add(reservation);

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        reservation.Slot = slot;
        return ToDto(reservation);
    }

    public async Task<bool> CancelAsync(Guid hotelId, Guid reservationId, CancellationToken ct = default)
    {
        var reservation = await db.Reservations
            .Include(r => r.Slot!).ThenInclude(s => s.Activity)
            .FirstOrDefaultAsync(r => r.Id == reservationId && r.Slot!.Activity!.HotelId == hotelId, ct);

        if (reservation is null || reservation.Status != ReservationStatus.Confirmed) return false;

        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledAt = DateTimeOffset.UtcNow;
        reservation.Slot!.BookedCount = Math.Max(0, reservation.Slot.BookedCount - 1);

        await db.SaveChangesAsync(ct);
        return true;
    }

    private static ReservationDto ToDto(Reservation r) => new(
        r.Id,
        r.SlotId,
        r.Slot!.ActivityId,
        r.Slot.Activity!.Name,
        r.Slot.StartTime,
        r.GuestName,
        r.RoomNumber,
        r.Status,
        r.CreatedAt);
}
