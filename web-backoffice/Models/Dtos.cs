namespace AllStay.Web.Backoffice.Models;

// Mirror AllStay.Domain.Enums — the API serializes enums as their underlying numbers, not strings.
public enum StaffRole { Manager = 0, FrontDesk = 1, ActivityCoordinator = 2 }
public enum ReservationStatus { Confirmed = 0, Cancelled = 1, CompletedNoShow = 2, Completed = 3 }

public record LoginRequest(string Email, string Password);
public record StaffProfileDto(Guid Id, string Email, string FullName, StaffRole Role, Guid HotelId, string HotelName);
public record LoginResponse(string Token, DateTimeOffset ExpiresAt, StaffProfileDto Staff);

public record ActivitySlotDto(Guid Id, DateTimeOffset StartTime, int Capacity, int BookedCount, int AvailableSpots);

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
    List<ActivitySlotDto> Slots);

public record CreateActivityRequest(string Name, string? Description, string Category, decimal Price, int DurationMinutes, string? ImageUrl);
public record UpdateActivityRequest(string Name, string? Description, string Category, decimal Price, int DurationMinutes, string? ImageUrl, bool IsActive);
public record CreateActivitySlotRequest(DateTimeOffset StartTime, int Capacity);

public record ReservationDto(
    Guid Id,
    Guid SlotId,
    Guid ActivityId,
    string ActivityName,
    DateTimeOffset SlotStartTime,
    string GuestName,
    string RoomNumber,
    ReservationStatus Status,
    DateTimeOffset CreatedAt);
