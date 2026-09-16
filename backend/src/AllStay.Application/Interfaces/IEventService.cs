using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IEventService
{
    Task<IReadOnlyList<EventDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default);
    Task<EventDto?> GetAsync(Guid hotelId, Guid eventId, CancellationToken ct = default);
    Task<EventDto> CreateAsync(Guid hotelId, CreateEventRequest request, CancellationToken ct = default);
    Task<EventDto?> UpdateAsync(Guid hotelId, Guid eventId, UpdateEventRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid hotelId, Guid eventId, CancellationToken ct = default);
}
