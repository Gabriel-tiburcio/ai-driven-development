namespace AllStay.Domain.Entities;

public class Restaurant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string CuisineType { get; set; }
    public required string Hours { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> MenuHighlights { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
