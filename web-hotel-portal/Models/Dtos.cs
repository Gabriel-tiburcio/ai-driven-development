namespace AllStay.Web.HotelPortal.Models;

// Mirror AllStay.Domain.Enums exactly — the API serializes enums as their underlying numbers.
public enum StaffRole { Manager = 0, FrontDesk = 1, ActivityCoordinator = 2 }
public enum ReservationStatus { Confirmed = 0, Cancelled = 1, CompletedNoShow = 2, Completed = 3 }
public enum HotelTier { Essential = 0, Professional = 1, Enterprise = 2 }
public enum GuestRequestStatus { Pending = 0, InProgress = 1, Done = 2 }
public enum ServiceRequestStatus { Pending = 0, Confirmed = 1, Cancelled = 2 }

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

public record HotelDto(Guid Id, string Name, string Code, HotelTier Tier, bool IsActive, string? Address, string? City);
public record CreateHotelRequest(string Name, string Code, HotelTier Tier, string? ContactEmail, string? ContactPhone, string? Address, string? City);
public record UpdateHotelLocationRequest(string? Address, string? City);
public record CreateStaffRequest(string Email, string Password, string FullName, StaffRole Role);
public record HotelStaffDto(Guid Id, string Email, string FullName, StaffRole Role, bool IsActive, DateTimeOffset CreatedAt);
public record UpdateActiveStatusRequest(bool IsActive);
public record LeadDto(Guid Id, string HotelName, string ContactName, string Email, string? Phone, string? Message, DateTimeOffset CreatedAt);

public record HotelInfoSectionDto(Guid Id, Guid HotelId, string Title, string Icon, string Content, int SortOrder, bool IsActive);
public record CreateHotelInfoSectionRequest(string Title, string Icon, string Content, int SortOrder);
public record UpdateHotelInfoSectionRequest(string Title, string Icon, string Content, int SortOrder, bool IsActive);

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

public record CreateEventRequest(string Name, string? Description, string Category, DateOnly EventDate, TimeOnly StartTime, string Location, string? ImageUrl);
public record UpdateEventRequest(string Name, string? Description, string Category, DateOnly EventDate, TimeOnly StartTime, string Location, string? ImageUrl, bool IsActive);

public record GuestRequestDto(
    Guid Id,
    Guid HotelId,
    string Type,
    string Details,
    string GuestName,
    string RoomNumber,
    GuestRequestStatus Status,
    DateTimeOffset CreatedAt);

public record UpdateGuestRequestStatusRequest(GuestRequestStatus Status);

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

public record CreateKidsActivityRequest(string Name, string? Description, string AgeRange, string Schedule, string Location, string? ImageUrl);
public record UpdateKidsActivityRequest(string Name, string? Description, string AgeRange, string Schedule, string Location, string? ImageUrl, bool IsActive);

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

public record CreateRestaurantRequest(string Name, string Description, string CuisineType, string Hours, string? ImageUrl, List<string> MenuHighlights);
public record UpdateRestaurantRequest(string Name, string Description, string CuisineType, string Hours, string? ImageUrl, List<string> MenuHighlights, bool IsActive);

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

public record CreateServiceItemRequest(string Name, string Description, string Category, decimal Price, int DurationMinutes, string? ImageUrl);
public record UpdateServiceItemRequest(string Name, string Description, string Category, decimal Price, int DurationMinutes, string? ImageUrl, bool IsActive);

public record ServiceRequestDto(
    Guid Id,
    Guid ServiceId,
    string ServiceName,
    string GuestName,
    string RoomNumber,
    string Status,
    DateTimeOffset CreatedAt);

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

public record CreateExternalExperienceRequest(string Name, string Description, string Category, decimal Price, string DurationLabel, string Location, string? ImageUrl);
public record UpdateExternalExperienceRequest(string Name, string Description, string Category, decimal Price, string DurationLabel, string Location, string? ImageUrl, bool IsActive);

public record ExperienceRequestDto(
    Guid Id,
    Guid ExternalExperienceId,
    string ExternalExperienceName,
    string GuestName,
    string RoomNumber,
    string Status,
    DateTimeOffset CreatedAt);

public record ConciergeKnowledgeEntryDto(Guid Id, Guid HotelId, string Title, string Content, DateTimeOffset CreatedAt);
public record CreateConciergeKnowledgeEntryRequest(string Title, string Content);
public record UpdateConciergeKnowledgeEntryRequest(string Title, string Content);
