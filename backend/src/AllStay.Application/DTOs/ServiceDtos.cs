namespace AllStay.Application.DTOs;

public record ServiceDto(
    Guid Id,
    Guid HotelId,
    string Name,
    string Description,
    string Category,
    decimal Price,
    int DurationMinutes,
    string? ImageUrl,
    bool IsActive);

public record CreateServiceItemRequest(
    string Name,
    string Description,
    string Category,
    decimal Price,
    int DurationMinutes,
    string? ImageUrl);

public record UpdateServiceItemRequest(
    string Name,
    string Description,
    string Category,
    decimal Price,
    int DurationMinutes,
    string? ImageUrl,
    bool IsActive);

public record ServiceRequestDto(
    Guid Id,
    Guid ServiceId,
    string ServiceName,
    string GuestName,
    string RoomNumber,
    string Status,
    DateTimeOffset CreatedAt);

public record CreateServiceHireRequest(string GuestName, string RoomNumber);
