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
    public string? Address { get; set; }
    public string? City { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<HotelStaffUser> StaffUsers { get; set; } = new List<HotelStaffUser>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<HotelInfoSection> InfoSections { get; set; } = new List<HotelInfoSection>();
    public ICollection<EventItem> Events { get; set; } = new List<EventItem>();
    public ICollection<GuestRequest> Requests { get; set; } = new List<GuestRequest>();
    public ICollection<KidsActivity> KidsActivities { get; set; } = new List<KidsActivity>();
    public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<ExternalExperience> ExternalExperiences { get; set; } = new List<ExternalExperience>();
    public ICollection<ConciergeKnowledgeEntry> ConciergeKnowledgeEntries { get; set; } = new List<ConciergeKnowledgeEntry>();
}
