namespace AllStay.Domain.Entities;

/// <summary>Captured from the institutional website's contact form for the B2B sales funnel.</summary>
public class Lead
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string HotelName { get; set; }
    public required string ContactName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public string? Message { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
