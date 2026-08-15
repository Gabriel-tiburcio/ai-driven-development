using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IHotelService
{
    Task<HotelDto?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<HotelDto> CreateAsync(CreateHotelRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<HotelDto>> ListAsync(CancellationToken ct = default);
    Task<StaffProfileDto?> CreateStaffAsync(Guid hotelId, CreateStaffRequest request, CancellationToken ct = default);
}
