using AllStay.Domain.Enums;

namespace AllStay.Domain.Entities;

public class ExperienceRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ExternalExperienceId { get; set; }
    public ExternalExperience? ExternalExperience { get; set; }

    public required string GuestName { get; set; }
    public required string RoomNumber { get; set; }
    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
