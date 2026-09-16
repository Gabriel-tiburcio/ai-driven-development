using AllStay.Domain.Enums;

namespace AllStay.Domain.Entities;

public class KidsEnrollment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid KidsActivityId { get; set; }
    public KidsActivity? KidsActivity { get; set; }

    public required string ChildName { get; set; }
    public required string ChildAge { get; set; }
    public required string GuardianRoomNumber { get; set; }
    public string? GuardianName { get; set; }
    public KidsEnrollmentStatus Status { get; set; } = KidsEnrollmentStatus.Confirmed;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
