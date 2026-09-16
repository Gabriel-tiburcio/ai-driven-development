using AllStay.Domain.Enums;

namespace AllStay.Application.DTOs;

public record GuestRequestDto(
    Guid Id,
    Guid HotelId,
    string Type,
    string Details,
    string GuestName,
    string RoomNumber,
    GuestRequestStatus Status,
    DateTimeOffset CreatedAt);

public record CreateGuestRequestRequest(string Type, string Details, string GuestName, string RoomNumber);

public record UpdateGuestRequestStatusRequest(GuestRequestStatus Status);
