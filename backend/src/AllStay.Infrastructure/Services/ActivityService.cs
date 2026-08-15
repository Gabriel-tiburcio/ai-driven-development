using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class ActivityService(AllStayDbContext db) : IActivityService
{
    public async Task<IReadOnlyList<ActivityDto>> ListForHotelAsync(Guid hotelId, string? category = null, CancellationToken ct = default)
    {
        var query = db.Activities
            .Include(a => a.Slots)
            .Where(a => a.HotelId == hotelId && a.IsActive);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(a => a.Category == category);

        var activities = await query.OrderBy(a => a.Name).ToListAsync(ct);
        return activities.Select(ToDto).ToList();
    }

    public async Task<ActivityDto?> GetAsync(Guid hotelId, Guid activityId, CancellationToken ct = default)
    {
        var activity = await db.Activities
            .Include(a => a.Slots)
            .FirstOrDefaultAsync(a => a.HotelId == hotelId && a.Id == activityId, ct);

        return activity is null ? null : ToDto(activity);
    }

    public async Task<ActivityDto> CreateAsync(Guid hotelId, CreateActivityRequest request, CancellationToken ct = default)
    {
        var activity = new Activity
        {
            HotelId = hotelId,
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Price = request.Price,
            DurationMinutes = request.DurationMinutes,
            ImageUrl = request.ImageUrl
        };

        db.Activities.Add(activity);
        await db.SaveChangesAsync(ct);
        return ToDto(activity);
    }

    public async Task<ActivityDto?> UpdateAsync(Guid hotelId, Guid activityId, UpdateActivityRequest request, CancellationToken ct = default)
    {
        var activity = await db.Activities
            .Include(a => a.Slots)
            .FirstOrDefaultAsync(a => a.HotelId == hotelId && a.Id == activityId, ct);

        if (activity is null) return null;

        activity.Name = request.Name;
        activity.Description = request.Description;
        activity.Category = request.Category;
        activity.Price = request.Price;
        activity.DurationMinutes = request.DurationMinutes;
        activity.ImageUrl = request.ImageUrl;
        activity.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return ToDto(activity);
    }

    public async Task<bool> DeleteAsync(Guid hotelId, Guid activityId, CancellationToken ct = default)
    {
        var activity = await db.Activities.FirstOrDefaultAsync(a => a.HotelId == hotelId && a.Id == activityId, ct);
        if (activity is null) return false;

        db.Activities.Remove(activity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<ActivitySlotDto?> AddSlotAsync(Guid hotelId, Guid activityId, CreateActivitySlotRequest request, CancellationToken ct = default)
    {
        var activity = await db.Activities.FirstOrDefaultAsync(a => a.HotelId == hotelId && a.Id == activityId, ct);
        if (activity is null) return null;

        var slot = new ActivitySlot
        {
            ActivityId = activityId,
            StartTime = request.StartTime,
            Capacity = request.Capacity
        };

        db.ActivitySlots.Add(slot);
        await db.SaveChangesAsync(ct);

        return new ActivitySlotDto(slot.Id, slot.StartTime, slot.Capacity, slot.BookedCount, slot.AvailableSpots);
    }

    public async Task<bool> DeleteSlotAsync(Guid hotelId, Guid activityId, Guid slotId, CancellationToken ct = default)
    {
        var slot = await db.ActivitySlots
            .Include(s => s.Activity)
            .FirstOrDefaultAsync(s => s.Id == slotId && s.ActivityId == activityId && s.Activity!.HotelId == hotelId, ct);

        if (slot is null) return false;

        db.ActivitySlots.Remove(slot);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static ActivityDto ToDto(Activity a) => new(
        a.Id,
        a.HotelId,
        a.Name,
        a.Description,
        a.Category,
        a.Price,
        a.DurationMinutes,
        a.ImageUrl,
        a.IsActive,
        a.Slots.OrderBy(s => s.StartTime)
            .Select(s => new ActivitySlotDto(s.Id, s.StartTime, s.Capacity, s.BookedCount, s.AvailableSpots))
            .ToList());
}
