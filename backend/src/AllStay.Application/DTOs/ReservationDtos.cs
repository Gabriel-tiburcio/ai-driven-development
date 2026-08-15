using AllStay.Domain.Enums;

namespace AllStay.Application.DTOs;

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

public record CreateReservationRequest(Guid SlotId, string GuestName, string RoomNumber);
