using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class RestaurantService(AllStayDbContext db) : IRestaurantService
{
    public async Task<IReadOnlyList<RestaurantDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var restaurants = await db.Restaurants
            .Where(r => r.HotelId == hotelId && r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync(ct);

        return restaurants.Select(ToDto).ToList();
    }

    public async Task<RestaurantDto?> GetAsync(Guid hotelId, Guid restaurantId, CancellationToken ct = default)
    {
        var restaurant = await db.Restaurants.FirstOrDefaultAsync(r => r.HotelId == hotelId && r.Id == restaurantId, ct);
        return restaurant is null ? null : ToDto(restaurant);
    }

    public async Task<RestaurantDto> CreateAsync(Guid hotelId, CreateRestaurantRequest request, CancellationToken ct = default)
    {
        var restaurant = new Restaurant
        {
            HotelId = hotelId,
            Name = request.Name,
            Description = request.Description,
            CuisineType = request.CuisineType,
            Hours = request.Hours,
            ImageUrl = request.ImageUrl,
            MenuHighlights = request.MenuHighlights
        };

        db.Restaurants.Add(restaurant);
        await db.SaveChangesAsync(ct);
        return ToDto(restaurant);
    }

    public async Task<RestaurantDto?> UpdateAsync(Guid hotelId, Guid restaurantId, UpdateRestaurantRequest request, CancellationToken ct = default)
    {
        var restaurant = await db.Restaurants.FirstOrDefaultAsync(r => r.HotelId == hotelId && r.Id == restaurantId, ct);
        if (restaurant is null) return null;

        restaurant.Name = request.Name;
        restaurant.Description = request.Description;
        restaurant.CuisineType = request.CuisineType;
        restaurant.Hours = request.Hours;
        restaurant.ImageUrl = request.ImageUrl;
        restaurant.MenuHighlights = request.MenuHighlights;
        restaurant.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return ToDto(restaurant);
    }

    public async Task<bool> DeleteAsync(Guid hotelId, Guid restaurantId, CancellationToken ct = default)
    {
        var restaurant = await db.Restaurants.FirstOrDefaultAsync(r => r.HotelId == hotelId && r.Id == restaurantId, ct);
        if (restaurant is null) return false;

        db.Restaurants.Remove(restaurant);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static RestaurantDto ToDto(Restaurant r) => new(
        r.Id,
        r.HotelId,
        r.Name,
        r.Description,
        r.CuisineType,
        r.Hours,
        r.ImageUrl,
        r.MenuHighlights,
        r.IsActive);
}
