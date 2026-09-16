using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IServiceService
{
    Task<IReadOnlyList<ServiceDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default);
    Task<ServiceDto?> GetAsync(Guid hotelId, Guid serviceId, CancellationToken ct = default);
    Task<ServiceDto> CreateAsync(Guid hotelId, CreateServiceItemRequest request, CancellationToken ct = default);
    Task<ServiceDto?> UpdateAsync(Guid hotelId, Guid serviceId, UpdateServiceItemRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid hotelId, Guid serviceId, CancellationToken ct = default);

    Task<ServiceRequestDto?> RequestAsync(Guid hotelId, Guid serviceId, CreateServiceHireRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceRequestDto>> ListRequestsForHotelAsync(Guid hotelId, CancellationToken ct = default);
}
