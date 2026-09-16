namespace AllStay.Application.DTOs;

public record RestaurantDto(
    Guid Id,
    Guid HotelId,
    string Name,
    string Description,
    string CuisineType,
    string Hours,
    string? ImageUrl,
    List<string> MenuHighlights,
    bool IsActive);

public record CreateRestaurantRequest(
    string Name,
    string Description,
    string CuisineType,
    string Hours,
    string? ImageUrl,
    List<string> MenuHighlights);

public record UpdateRestaurantRequest(
    string Name,
    string Description,
    string CuisineType,
    string Hours,
    string? ImageUrl,
    List<string> MenuHighlights,
    bool IsActive);
