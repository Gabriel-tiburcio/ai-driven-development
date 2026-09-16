using AllStay.Domain.Enums;

namespace AllStay.Domain.Entities;

public class ServiceRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ServiceId { get; set; }
    public Service? Service { get; set; }

    public required string GuestName { get; set; }
    public required string RoomNumber { get; set; }
    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
