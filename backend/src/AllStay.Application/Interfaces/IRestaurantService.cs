using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IRestaurantService
{
    Task<IReadOnlyList<RestaurantDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default);
    Task<RestaurantDto?> GetAsync(Guid hotelId, Guid restaurantId, CancellationToken ct = default);
    Task<RestaurantDto> CreateAsync(Guid hotelId, CreateRestaurantRequest request, CancellationToken ct = default);
    Task<RestaurantDto?> UpdateAsync(Guid hotelId, Guid restaurantId, UpdateRestaurantRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid hotelId, Guid restaurantId, CancellationToken ct = default);
}
