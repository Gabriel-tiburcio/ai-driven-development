namespace AllStay.Application.DTOs;

public record ExternalExperienceDto(
    Guid Id,
    Guid HotelId,
    string Name,
    string Description,
    string Category,
    decimal Price,
    string DurationLabel,
    string Location,
    string? ImageUrl,
    bool IsActive);

public record CreateExternalExperienceRequest(
    string Name,
    string Description,
    string Category,
    decimal Price,
    string DurationLabel,
    string Location,
    string? ImageUrl);

public record UpdateExternalExperienceRequest(
    string Name,
    string Description,
    string Category,
    decimal Price,
    string DurationLabel,
    string Location,
    string? ImageUrl,
    bool IsActive);

public record ExperienceRequestDto(
    Guid Id,
    Guid ExternalExperienceId,
    string ExternalExperienceName,
    string GuestName,
    string RoomNumber,
    string Status,
    DateTimeOffset CreatedAt);

public record CreateExperienceHireRequest(string GuestName, string RoomNumber);
