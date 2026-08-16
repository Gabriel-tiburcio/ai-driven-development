using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IHotelService
{
    Task<HotelDto?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<HotelDto> CreateAsync(CreateHotelRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<HotelDto>> ListAsync(CancellationToken ct = default);
    Task<StaffProfileDto?> CreateStaffAsync(Guid hotelId, CreateStaffRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<HotelStaffDto>> ListStaffAsync(Guid hotelId, CancellationToken ct = default);
    Task<bool> SetHotelActiveAsync(Guid hotelId, bool isActive, CancellationToken ct = default);
    Task<bool> SetStaffActiveAsync(Guid hotelId, Guid staffId, bool isActive, CancellationToken ct = default);
}
