using AllStay.Domain.Enums;

namespace AllStay.Application.DTOs;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string Token, DateTimeOffset ExpiresAt, StaffProfileDto Staff);

public record StaffProfileDto(Guid Id, string Email, string FullName, StaffRole Role, Guid HotelId, string HotelName);
