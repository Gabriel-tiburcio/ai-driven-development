using AllStay.Domain.Enums;

namespace AllStay.Domain.Entities;

public class HotelStaffUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string FullName { get; set; }
    public StaffRole Role { get; set; } = StaffRole.FrontDesk;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
