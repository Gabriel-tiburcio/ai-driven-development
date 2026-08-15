using AllStay.Domain.Enums;

namespace AllStay.Domain.Entities;

public class Hotel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }

    /// <summary>Short code guests reach via QR code / link, e.g. allstay.app/h/{Code}.</summary>
    public required string Code { get; set; }

    public HotelTier Tier { get; set; } = HotelTier.Essential;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<HotelStaffUser> StaffUsers { get; set; } = new List<HotelStaffUser>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
