using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class EventService(AllStayDbContext db) : IEventService
{
    public async Task<IReadOnlyList<EventDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var events = await db.Events
            .Where(e => e.HotelId == hotelId && e.IsActive)
            .OrderBy(e => e.EventDate)
            .ThenBy(e => e.StartTime)
            .ToListAsync(ct);

        return events.Select(ToDto).ToList();
    }

    public async Task<EventDto?> GetAsync(Guid hotelId, Guid eventId, CancellationToken ct = default)
    {
        var evt = await db.Events
            .FirstOrDefaultAsync(e => e.HotelId == hotelId && e.Id == eventId, ct);

        return evt is null ? null : ToDto(evt);
    }

    public async Task<EventDto> CreateAsync(Guid hotelId, CreateEventRequest request, CancellationToken ct = default)
    {
        var evt = new EventItem
        {
            HotelId = hotelId,
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            EventDate = request.EventDate,
            StartTime = request.StartTime,
            Location = request.Location,
            ImageUrl = request.ImageUrl
        };

        db.Events.Add(evt);
        await db.SaveChangesAsync(ct);
        return ToDto(evt);
    }

    public async Task<EventDto?> UpdateAsync(Guid hotelId, Guid eventId, UpdateEventRequest request, CancellationToken ct = default)
    {
        var evt = await db.Events
            .FirstOrDefaultAsync(e => e.HotelId == hotelId && e.Id == eventId, ct);

        if (evt is null) return null;

        evt.Name = request.Name;
        evt.Description = request.Description;
        evt.Category = request.Category;
        evt.EventDate = request.EventDate;
        evt.StartTime = request.StartTime;
        evt.Location = request.Location;
        evt.ImageUrl = request.ImageUrl;
        evt.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return ToDto(evt);
    }

    public async Task<bool> DeleteAsync(Guid hotelId, Guid eventId, CancellationToken ct = default)
    {
        var evt = await db.Events.FirstOrDefaultAsync(e => e.HotelId == hotelId && e.Id == eventId, ct);
        if (evt is null) return false;

        db.Events.Remove(evt);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static EventDto ToDto(EventItem e) => new(
        e.Id,
        e.HotelId,
        e.Name,
        e.Description,
        e.Category,
        e.EventDate,
        e.StartTime,
        e.Location,
        e.ImageUrl,
        e.IsActive);
}
