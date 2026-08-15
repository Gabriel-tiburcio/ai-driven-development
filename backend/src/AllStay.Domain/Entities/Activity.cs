namespace AllStay.Domain.Entities;

public class Activity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Category { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<ActivitySlot> Slots { get; set; } = new List<ActivitySlot>();
}
