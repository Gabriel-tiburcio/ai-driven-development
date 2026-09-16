namespace AllStay.Application.DTOs;

public record KidsActivityDto(
    Guid Id,
    Guid HotelId,
    string Name,
    string? Description,
    string AgeRange,
    string Schedule,
    string Location,
    string? ImageUrl,
    bool IsActive);

public record CreateKidsActivityRequest(
    string Name,
    string? Description,
    string AgeRange,
    string Schedule,
    string Location,
    string? ImageUrl);

public record UpdateKidsActivityRequest(
    string Name,
    string? Description,
    string AgeRange,
    string Schedule,
    string Location,
    string? ImageUrl,
    bool IsActive);

public record KidsEnrollmentDto(
    Guid Id,
    Guid KidsActivityId,
    string KidsActivityName,
    string ChildName,
    string ChildAge,
    string GuardianRoomNumber,
    string? GuardianName,
    string Status,
    DateTimeOffset CreatedAt);

public record CreateKidsEnrollmentRequest(string ChildName, string ChildAge, string GuardianRoomNumber, string? GuardianName);
