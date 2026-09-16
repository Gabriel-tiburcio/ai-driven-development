using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IKidsActivityService
{
    Task<IReadOnlyList<KidsActivityDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default);
    Task<KidsActivityDto?> GetAsync(Guid hotelId, Guid kidsActivityId, CancellationToken ct = default);
    Task<KidsActivityDto> CreateAsync(Guid hotelId, CreateKidsActivityRequest request, CancellationToken ct = default);
    Task<KidsActivityDto?> UpdateAsync(Guid hotelId, Guid kidsActivityId, UpdateKidsActivityRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid hotelId, Guid kidsActivityId, CancellationToken ct = default);

    Task<KidsEnrollmentDto?> EnrollAsync(Guid hotelId, Guid kidsActivityId, CreateKidsEnrollmentRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<KidsEnrollmentDto>> ListEnrollmentsForHotelAsync(Guid hotelId, CancellationToken ct = default);
}
