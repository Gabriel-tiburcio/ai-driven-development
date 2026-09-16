namespace AllStay.Domain.Entities;

public class KidsActivity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string AgeRange { get; set; }
    public required string Schedule { get; set; }
    public required string Location { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<KidsEnrollment> Enrollments { get; set; } = new List<KidsEnrollment>();
}
