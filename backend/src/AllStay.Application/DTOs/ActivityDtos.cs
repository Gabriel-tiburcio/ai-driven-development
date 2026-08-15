namespace AllStay.Application.DTOs;

public record ActivityDto(
    Guid Id,
    Guid HotelId,
    string Name,
    string? Description,
    string Category,
    decimal Price,
    int DurationMinutes,
    string? ImageUrl,
    bool IsActive,
    IReadOnlyList<ActivitySlotDto> Slots);

public record ActivitySlotDto(Guid Id, DateTimeOffset StartTime, int Capacity, int BookedCount, int AvailableSpots);

public record CreateActivityRequest(
    string Name,
    string? Description,
    string Category,
    decimal Price,
    int DurationMinutes,
    string? ImageUrl);

public record UpdateActivityRequest(
    string Name,
    string? Description,
    string Category,
    decimal Price,
    int DurationMinutes,
    string? ImageUrl,
    bool IsActive);

public record CreateActivitySlotRequest(DateTimeOffset StartTime, int Capacity);
