using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IExternalExperienceService
{
    Task<IReadOnlyList<ExternalExperienceDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default);
    Task<ExternalExperienceDto?> GetAsync(Guid hotelId, Guid experienceId, CancellationToken ct = default);
    Task<ExternalExperienceDto> CreateAsync(Guid hotelId, CreateExternalExperienceRequest request, CancellationToken ct = default);
    Task<ExternalExperienceDto?> UpdateAsync(Guid hotelId, Guid experienceId, UpdateExternalExperienceRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid hotelId, Guid experienceId, CancellationToken ct = default);

    Task<ExperienceRequestDto?> RequestAsync(Guid hotelId, Guid experienceId, CreateExperienceHireRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<ExperienceRequestDto>> ListRequestsForHotelAsync(Guid hotelId, CancellationToken ct = default);
}
