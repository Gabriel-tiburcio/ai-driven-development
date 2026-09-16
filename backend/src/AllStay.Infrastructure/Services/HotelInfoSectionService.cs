using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class HotelInfoSectionService(AllStayDbContext db) : IHotelInfoSectionService
{
    public async Task<IReadOnlyList<HotelInfoSectionDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var sections = await db.HotelInfoSections
            .Where(s => s.HotelId == hotelId && s.IsActive)
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Title)
            .ToListAsync(ct);

        return sections.Select(ToDto).ToList();
    }

    public async Task<HotelInfoSectionDto?> GetAsync(Guid hotelId, Guid sectionId, CancellationToken ct = default)
    {
        var section = await db.HotelInfoSections
            .FirstOrDefaultAsync(s => s.HotelId == hotelId && s.Id == sectionId, ct);

        return section is null ? null : ToDto(section);
    }

    public async Task<HotelInfoSectionDto> CreateAsync(Guid hotelId, CreateHotelInfoSectionRequest request, CancellationToken ct = default)
    {
        var section = new HotelInfoSection
        {
            HotelId = hotelId,
            Title = request.Title,
            Icon = request.Icon,
            Content = request.Content,
            SortOrder = request.SortOrder
        };

        db.HotelInfoSections.Add(section);
        await db.SaveChangesAsync(ct);
        return ToDto(section);
    }

    public async Task<HotelInfoSectionDto?> UpdateAsync(Guid hotelId, Guid sectionId, UpdateHotelInfoSectionRequest request, CancellationToken ct = default)
    {
        var section = await db.HotelInfoSections
            .FirstOrDefaultAsync(s => s.HotelId == hotelId && s.Id == sectionId, ct);

        if (section is null) return null;

        section.Title = request.Title;
        section.Icon = request.Icon;
        section.Content = request.Content;
        section.SortOrder = request.SortOrder;
        section.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return ToDto(section);
    }

    public async Task<bool> DeleteAsync(Guid hotelId, Guid sectionId, CancellationToken ct = default)
    {
        var section = await db.HotelInfoSections.FirstOrDefaultAsync(s => s.HotelId == hotelId && s.Id == sectionId, ct);
        if (section is null) return false;

        db.HotelInfoSections.Remove(section);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static HotelInfoSectionDto ToDto(HotelInfoSection s) => new(
        s.Id,
        s.HotelId,
        s.Title,
        s.Icon,
        s.Content,
        s.SortOrder,
        s.IsActive);
}
