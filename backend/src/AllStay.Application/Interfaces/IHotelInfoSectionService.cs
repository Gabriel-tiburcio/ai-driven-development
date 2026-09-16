using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IHotelInfoSectionService
{
    Task<IReadOnlyList<HotelInfoSectionDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default);
    Task<HotelInfoSectionDto?> GetAsync(Guid hotelId, Guid sectionId, CancellationToken ct = default);
    Task<HotelInfoSectionDto> CreateAsync(Guid hotelId, CreateHotelInfoSectionRequest request, CancellationToken ct = default);
    Task<HotelInfoSectionDto?> UpdateAsync(Guid hotelId, Guid sectionId, UpdateHotelInfoSectionRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid hotelId, Guid sectionId, CancellationToken ct = default);
}
