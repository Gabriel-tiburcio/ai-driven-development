using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IActivityService
{
    Task<IReadOnlyList<ActivityDto>> ListForHotelAsync(Guid hotelId, string? category = null, CancellationToken ct = default);
    Task<ActivityDto?> GetAsync(Guid hotelId, Guid activityId, CancellationToken ct = default);
    Task<ActivityDto> CreateAsync(Guid hotelId, CreateActivityRequest request, CancellationToken ct = default);
    Task<ActivityDto?> UpdateAsync(Guid hotelId, Guid activityId, UpdateActivityRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid hotelId, Guid activityId, CancellationToken ct = default);

    Task<ActivitySlotDto?> AddSlotAsync(Guid hotelId, Guid activityId, CreateActivitySlotRequest request, CancellationToken ct = default);
    Task<bool> DeleteSlotAsync(Guid hotelId, Guid activityId, Guid slotId, CancellationToken ct = default);
}
