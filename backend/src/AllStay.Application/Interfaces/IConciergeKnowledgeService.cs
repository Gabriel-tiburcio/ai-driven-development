using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IConciergeKnowledgeService
{
    Task<IReadOnlyList<ConciergeKnowledgeEntryDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default);
    Task<ConciergeKnowledgeEntryDto> CreateAsync(Guid hotelId, CreateConciergeKnowledgeEntryRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid hotelId, Guid entryId, CancellationToken ct = default);
}
