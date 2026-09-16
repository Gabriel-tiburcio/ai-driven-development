namespace AllStay.Application.DTOs;

public record EventDto(
    Guid Id,
    Guid HotelId,
    string Name,
    string? Description,
    string Category,
    DateOnly EventDate,
    TimeOnly StartTime,
    string Location,
    string? ImageUrl,
    bool IsActive);

public record CreateEventRequest(
    string Name,
    string? Description,
    string Category,
    DateOnly EventDate,
    TimeOnly StartTime,
    string Location,
    string? ImageUrl);

public record UpdateEventRequest(
    string Name,
    string? Description,
    string Category,
    DateOnly EventDate,
    TimeOnly StartTime,
    string Location,
    string? ImageUrl,
    bool IsActive);
