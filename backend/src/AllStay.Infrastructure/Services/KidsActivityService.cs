using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class KidsActivityService(AllStayDbContext db) : IKidsActivityService
{
    public async Task<IReadOnlyList<KidsActivityDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var activities = await db.KidsActivities
            .Where(k => k.HotelId == hotelId && k.IsActive)
            .OrderBy(k => k.Name)
            .ToListAsync(ct);

        return activities.Select(ToDto).ToList();
    }

    public async Task<KidsActivityDto?> GetAsync(Guid hotelId, Guid kidsActivityId, CancellationToken ct = default)
    {
        var activity = await db.KidsActivities
            .FirstOrDefaultAsync(k => k.HotelId == hotelId && k.Id == kidsActivityId, ct);

        return activity is null ? null : ToDto(activity);
    }

    public async Task<KidsActivityDto> CreateAsync(Guid hotelId, CreateKidsActivityRequest request, CancellationToken ct = default)
    {
        var activity = new KidsActivity
        {
            HotelId = hotelId,
            Name = request.Name,
            Description = request.Description,
            AgeRange = request.AgeRange,
            Schedule = request.Schedule,
            Location = request.Location,
            ImageUrl = request.ImageUrl
        };

        db.KidsActivities.Add(activity);
        await db.SaveChangesAsync(ct);
        return ToDto(activity);
    }

    public async Task<KidsActivityDto?> UpdateAsync(Guid hotelId, Guid kidsActivityId, UpdateKidsActivityRequest request, CancellationToken ct = default)
    {
        var activity = await db.KidsActivities.FirstOrDefaultAsync(k => k.HotelId == hotelId && k.Id == kidsActivityId, ct);
        if (activity is null) return null;

        activity.Name = request.Name;
        activity.Description = request.Description;
        activity.AgeRange = request.AgeRange;
        activity.Schedule = request.Schedule;
        activity.Location = request.Location;
        activity.ImageUrl = request.ImageUrl;
        activity.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return ToDto(activity);
    }

    public async Task<bool> DeleteAsync(Guid hotelId, Guid kidsActivityId, CancellationToken ct = default)
    {
        var activity = await db.KidsActivities.FirstOrDefaultAsync(k => k.HotelId == hotelId && k.Id == kidsActivityId, ct);
        if (activity is null) return false;

        db.KidsActivities.Remove(activity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<KidsEnrollmentDto?> EnrollAsync(Guid hotelId, Guid kidsActivityId, CreateKidsEnrollmentRequest request, CancellationToken ct = default)
    {
        var activity = await db.KidsActivities.FirstOrDefaultAsync(k => k.HotelId == hotelId && k.Id == kidsActivityId, ct);
        if (activity is null) return null;

        var enrollment = new KidsEnrollment
        {
            KidsActivityId = kidsActivityId,
            ChildName = request.ChildName,
            ChildAge = request.ChildAge,
            GuardianRoomNumber = request.GuardianRoomNumber,
            GuardianName = request.GuardianName
        };

        db.KidsEnrollments.Add(enrollment);
        await db.SaveChangesAsync(ct);

        return new KidsEnrollmentDto(
            enrollment.Id,
            enrollment.KidsActivityId,
            activity.Name,
            enrollment.ChildName,
            enrollment.ChildAge,
            enrollment.GuardianRoomNumber,
            enrollment.GuardianName,
            enrollment.Status.ToString(),
            enrollment.CreatedAt);
    }

    public async Task<IReadOnlyList<KidsEnrollmentDto>> ListEnrollmentsForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var enrollments = await db.KidsEnrollments
            .Include(e => e.KidsActivity)
            .Where(e => e.KidsActivity!.HotelId == hotelId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);

        return enrollments.Select(e => new KidsEnrollmentDto(
            e.Id,
            e.KidsActivityId,
            e.KidsActivity!.Name,
            e.ChildName,
            e.ChildAge,
            e.GuardianRoomNumber,
            e.GuardianName,
            e.Status.ToString(),
            e.CreatedAt)).ToList();
    }

    private static KidsActivityDto ToDto(KidsActivity k) => new(
        k.Id,
        k.HotelId,
        k.Name,
        k.Description,
        k.AgeRange,
        k.Schedule,
        k.Location,
        k.ImageUrl,
        k.IsActive);
}
