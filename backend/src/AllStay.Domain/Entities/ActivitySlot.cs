namespace AllStay.Domain.Entities;

public class ActivitySlot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ActivityId { get; set; }
    public Activity? Activity { get; set; }

    public DateTimeOffset StartTime { get; set; }
    public int Capacity { get; set; }
    public int BookedCount { get; set; }

    public int AvailableSpots => Capacity - BookedCount;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
