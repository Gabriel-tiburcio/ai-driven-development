using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class ExternalExperienceService(AllStayDbContext db) : IExternalExperienceService
{
    public async Task<IReadOnlyList<ExternalExperienceDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var experiences = await db.ExternalExperiences
            .Where(e => e.HotelId == hotelId && e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync(ct);

        return experiences.Select(ToDto).ToList();
    }

    public async Task<ExternalExperienceDto?> GetAsync(Guid hotelId, Guid experienceId, CancellationToken ct = default)
    {
        var experience = await db.ExternalExperiences.FirstOrDefaultAsync(e => e.HotelId == hotelId && e.Id == experienceId, ct);
        return experience is null ? null : ToDto(experience);
    }

    public async Task<ExternalExperienceDto> CreateAsync(Guid hotelId, CreateExternalExperienceRequest request, CancellationToken ct = default)
    {
        var experience = new ExternalExperience
        {
            HotelId = hotelId,
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Price = request.Price,
            DurationLabel = request.DurationLabel,
            Location = request.Location,
            ImageUrl = request.ImageUrl
        };

        db.ExternalExperiences.Add(experience);
        await db.SaveChangesAsync(ct);
        return ToDto(experience);
    }

    public async Task<ExternalExperienceDto?> UpdateAsync(Guid hotelId, Guid experienceId, UpdateExternalExperienceRequest request, CancellationToken ct = default)
    {
        var experience = await db.ExternalExperiences.FirstOrDefaultAsync(e => e.HotelId == hotelId && e.Id == experienceId, ct);
        if (experience is null) return null;

        experience.Name = request.Name;
        experience.Description = request.Description;
        experience.Category = request.Category;
        experience.Price = request.Price;
        experience.DurationLabel = request.DurationLabel;
        experience.Location = request.Location;
        experience.ImageUrl = request.ImageUrl;
        experience.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return ToDto(experience);
    }

    public async Task<bool> DeleteAsync(Guid hotelId, Guid experienceId, CancellationToken ct = default)
    {
        var experience = await db.ExternalExperiences.FirstOrDefaultAsync(e => e.HotelId == hotelId && e.Id == experienceId, ct);
        if (experience is null) return false;

        db.ExternalExperiences.Remove(experience);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<ExperienceRequestDto?> RequestAsync(Guid hotelId, Guid experienceId, CreateExperienceHireRequest request, CancellationToken ct = default)
    {
        var experience = await db.ExternalExperiences.FirstOrDefaultAsync(e => e.HotelId == hotelId && e.Id == experienceId, ct);
        if (experience is null) return null;

        var experienceRequest = new ExperienceRequest
        {
            ExternalExperienceId = experienceId,
            GuestName = request.GuestName,
            RoomNumber = request.RoomNumber
        };

        db.ExperienceRequests.Add(experienceRequest);
        await db.SaveChangesAsync(ct);

        return new ExperienceRequestDto(
            experienceRequest.Id,
            experienceRequest.ExternalExperienceId,
            experience.Name,
            experienceRequest.GuestName,
            experienceRequest.RoomNumber,
            experienceRequest.Status.ToString(),
            experienceRequest.CreatedAt);
    }

    public async Task<IReadOnlyList<ExperienceRequestDto>> ListRequestsForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var requests = await db.ExperienceRequests
            .Include(r => r.ExternalExperience)
            .Where(r => r.ExternalExperience!.HotelId == hotelId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return requests.Select(r => new ExperienceRequestDto(
            r.Id,
            r.ExternalExperienceId,
            r.ExternalExperience!.Name,
            r.GuestName,
            r.RoomNumber,
            r.Status.ToString(),
            r.CreatedAt)).ToList();
    }

    private static ExternalExperienceDto ToDto(ExternalExperience e) => new(
        e.Id,
        e.HotelId,
        e.Name,
        e.Description,
        e.Category,
        e.Price,
        e.DurationLabel,
        e.Location,
        e.ImageUrl,
        e.IsActive);
}
