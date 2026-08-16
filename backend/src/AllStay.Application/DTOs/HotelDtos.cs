using AllStay.Domain.Enums;

namespace AllStay.Application.DTOs;

public record HotelDto(Guid Id, string Name, string Code, HotelTier Tier, bool IsActive);

public record CreateHotelRequest(string Name, string Code, HotelTier Tier, string? ContactEmail, string? ContactPhone);

public record CreateStaffRequest(string Email, string Password, string FullName, StaffRole Role);

public record HotelStaffDto(Guid Id, string Email, string FullName, StaffRole Role, bool IsActive, DateTimeOffset CreatedAt);

public record UpdateActiveStatusRequest(bool IsActive);
