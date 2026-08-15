using AllStay.Domain.Enums;

namespace AllStay.Domain.Entities;

/// <summary>
/// MVP1 has no digital payment: reservations are charged to the guest's room bill at checkout.
/// PaymentStatus is a placeholder extension point for the future gateway-integrated MVP2.
/// </summary>
public class Reservation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SlotId { get; set; }
    public ActivitySlot? Slot { get; set; }

    public required string GuestName { get; set; }
    public required string RoomNumber { get; set; }
    public bool ChargeToRoom { get; set; } = true;

    public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CancelledAt { get; set; }
}
