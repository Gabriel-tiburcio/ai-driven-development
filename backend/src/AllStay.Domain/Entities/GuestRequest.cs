using AllStay.Domain.Enums;

namespace AllStay.Domain.Entities;

public class GuestRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public required string Type { get; set; }
    public required string Details { get; set; }
    public required string GuestName { get; set; }
    public required string RoomNumber { get; set; }
    public GuestRequestStatus Status { get; set; } = GuestRequestStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
