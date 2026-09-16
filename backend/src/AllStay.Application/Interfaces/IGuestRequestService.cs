using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IGuestRequestService
{
    Task<IReadOnlyList<GuestRequestDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default);
    Task<IReadOnlyList<GuestRequestDto>> ListForGuestAsync(Guid hotelId, string roomNumber, CancellationToken ct = default);
    Task<GuestRequestDto> CreateAsync(Guid hotelId, CreateGuestRequestRequest request, CancellationToken ct = default);
    Task<GuestRequestDto?> UpdateStatusAsync(Guid hotelId, Guid requestId, UpdateGuestRequestStatusRequest request, CancellationToken ct = default);
}
