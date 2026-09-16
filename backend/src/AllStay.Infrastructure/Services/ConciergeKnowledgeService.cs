using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class ConciergeKnowledgeService(AllStayDbContext db) : IConciergeKnowledgeService
{
    public async Task<IReadOnlyList<ConciergeKnowledgeEntryDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var entries = await db.ConciergeKnowledgeEntries
            .Where(e => e.HotelId == hotelId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);

        return entries.Select(ToDto).ToList();
    }

    public async Task<ConciergeKnowledgeEntryDto> CreateAsync(Guid hotelId, CreateConciergeKnowledgeEntryRequest request, CancellationToken ct = default)
    {
        var entry = new ConciergeKnowledgeEntry
        {
            HotelId = hotelId,
            Title = request.Title,
            Content = request.Content
        };

        db.ConciergeKnowledgeEntries.Add(entry);
        await db.SaveChangesAsync(ct);
        return ToDto(entry);
    }

    public async Task<ConciergeKnowledgeEntryDto?> UpdateAsync(Guid hotelId, Guid entryId, UpdateConciergeKnowledgeEntryRequest request, CancellationToken ct = default)
    {
        var entry = await db.ConciergeKnowledgeEntries.FirstOrDefaultAsync(e => e.HotelId == hotelId && e.Id == entryId, ct);
        if (entry is null) return null;

        entry.Title = request.Title;
        entry.Content = request.Content;
        await db.SaveChangesAsync(ct);
        return ToDto(entry);
    }

    public async Task<bool> DeleteAsync(Guid hotelId, Guid entryId, CancellationToken ct = default)
    {
        var entry = await db.ConciergeKnowledgeEntries.FirstOrDefaultAsync(e => e.HotelId == hotelId && e.Id == entryId, ct);
        if (entry is null) return false;

        db.ConciergeKnowledgeEntries.Remove(entry);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static ConciergeKnowledgeEntryDto ToDto(ConciergeKnowledgeEntry e) => new(e.Id, e.HotelId, e.Title, e.Content, e.CreatedAt);
}
