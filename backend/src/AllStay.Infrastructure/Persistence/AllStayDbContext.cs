using System.Text.Json;
using AllStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AllStay.Infrastructure.Persistence;

public class AllStayDbContext(DbContextOptions<AllStayDbContext> options) : DbContext(options)
{
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<HotelStaffUser> HotelStaffUsers => Set<HotelStaffUser>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<ActivitySlot> ActivitySlots => Set<ActivitySlot>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<HotelInfoSection> HotelInfoSections => Set<HotelInfoSection>();
    public DbSet<EventItem> Events => Set<EventItem>();
    public DbSet<GuestRequest> GuestRequests => Set<GuestRequest>();
    public DbSet<KidsActivity> KidsActivities => Set<KidsActivity>();
    public DbSet<KidsEnrollment> KidsEnrollments => Set<KidsEnrollment>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<ExternalExperience> ExternalExperiences => Set<ExternalExperience>();
    public DbSet<ExperienceRequest> ExperienceRequests => Set<ExperienceRequest>();
    public DbSet<ConciergeKnowledgeEntry> ConciergeKnowledgeEntries => Set<ConciergeKnowledgeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hotel>(e =>
        {
            e.HasIndex(h => h.Code).IsUnique();
            e.Property(h => h.Name).HasMaxLength(200).IsRequired();
            e.Property(h => h.Code).HasMaxLength(50).IsRequired();
            e.Property(h => h.Address).HasMaxLength(300);
            e.Property(h => h.City).HasMaxLength(120);
        });

        modelBuilder.Entity<HotelStaffUser>(e =>
        {
            e.HasIndex(s => s.Email).IsUnique();
            e.Property(s => s.Email).HasMaxLength(200).IsRequired();
            e.HasOne(s => s.Hotel)
                .WithMany(h => h.StaffUsers)
                .HasForeignKey(s => s.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Activity>(e =>
        {
            e.Property(a => a.Name).HasMaxLength(200).IsRequired();
            e.Property(a => a.Category).HasMaxLength(100).IsRequired();
            e.Property(a => a.Price).HasColumnType("decimal(10,2)");
            e.HasOne(a => a.Hotel)
                .WithMany(h => h.Activities)
                .HasForeignKey(a => a.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActivitySlot>(e =>
        {
            e.HasOne(s => s.Activity)
                .WithMany(a => a.Slots)
                .HasForeignKey(s => s.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Reservation>(e =>
        {
            e.Property(r => r.GuestName).HasMaxLength(200).IsRequired();
            e.Property(r => r.RoomNumber).HasMaxLength(20).IsRequired();
            e.HasOne(r => r.Slot)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SlotId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Lead>(e =>
        {
            e.Property(l => l.HotelName).HasMaxLength(200).IsRequired();
            e.Property(l => l.ContactName).HasMaxLength(200).IsRequired();
            e.Property(l => l.Email).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<HotelInfoSection>(e =>
        {
            e.Property(s => s.Title).HasMaxLength(200).IsRequired();
            e.Property(s => s.Icon).HasMaxLength(20).IsRequired();
            e.HasOne(s => s.Hotel)
                .WithMany(h => h.InfoSections)
                .HasForeignKey(s => s.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EventItem>(e =>
        {
            e.Property(ev => ev.Name).HasMaxLength(200).IsRequired();
            e.Property(ev => ev.Category).HasMaxLength(100).IsRequired();
            e.Property(ev => ev.Location).HasMaxLength(200).IsRequired();
            e.HasOne(ev => ev.Hotel)
                .WithMany(h => h.Events)
                .HasForeignKey(ev => ev.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GuestRequest>(e =>
        {
            e.Property(r => r.Type).HasMaxLength(100).IsRequired();
            e.Property(r => r.GuestName).HasMaxLength(200).IsRequired();
            e.Property(r => r.RoomNumber).HasMaxLength(20).IsRequired();
            e.HasOne(r => r.Hotel)
                .WithMany(h => h.Requests)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<KidsActivity>(e =>
        {
            e.Property(k => k.Name).HasMaxLength(200).IsRequired();
            e.Property(k => k.AgeRange).HasMaxLength(50).IsRequired();
            e.Property(k => k.Schedule).HasMaxLength(200).IsRequired();
            e.Property(k => k.Location).HasMaxLength(200).IsRequired();
            e.HasOne(k => k.Hotel)
                .WithMany(h => h.KidsActivities)
                .HasForeignKey(k => k.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<KidsEnrollment>(e =>
        {
            e.Property(en => en.ChildName).HasMaxLength(200).IsRequired();
            e.Property(en => en.ChildAge).HasMaxLength(20).IsRequired();
            e.Property(en => en.GuardianRoomNumber).HasMaxLength(20).IsRequired();
            e.Property(en => en.GuardianName).HasMaxLength(200);
            e.HasOne(en => en.KidsActivity)
                .WithMany(k => k.Enrollments)
                .HasForeignKey(en => en.KidsActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Restaurant>(e =>
        {
            e.Property(r => r.Name).HasMaxLength(200).IsRequired();
            e.Property(r => r.CuisineType).HasMaxLength(100).IsRequired();
            e.Property(r => r.Hours).HasMaxLength(200).IsRequired();
            var menuHighlightsComparer = new ValueComparer<List<string>>(
                (a, b) => a!.SequenceEqual(b!),
                v => v.Aggregate(0, (hash, s) => HashCode.Combine(hash, s.GetHashCode())),
                v => v.ToList());
            e.Property(r => r.MenuHighlights)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new())
                .Metadata.SetValueComparer(menuHighlightsComparer);
            e.HasOne(r => r.Hotel)
                .WithMany(h => h.Restaurants)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Service>(e =>
        {
            e.Property(s => s.Name).HasMaxLength(200).IsRequired();
            e.Property(s => s.Category).HasMaxLength(100).IsRequired();
            e.Property(s => s.Price).HasColumnType("decimal(10,2)");
            e.HasOne(s => s.Hotel)
                .WithMany(h => h.Services)
                .HasForeignKey(s => s.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ServiceRequest>(e =>
        {
            e.Property(r => r.GuestName).HasMaxLength(200).IsRequired();
            e.Property(r => r.RoomNumber).HasMaxLength(20).IsRequired();
            e.HasOne(r => r.Service)
                .WithMany(s => s.Requests)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExternalExperience>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Category).HasMaxLength(100).IsRequired();
            e.Property(x => x.DurationLabel).HasMaxLength(100).IsRequired();
            e.Property(x => x.Location).HasMaxLength(200).IsRequired();
            e.Property(x => x.Price).HasColumnType("decimal(10,2)");
            e.HasOne(x => x.Hotel)
                .WithMany(h => h.ExternalExperiences)
                .HasForeignKey(x => x.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExperienceRequest>(e =>
        {
            e.Property(r => r.GuestName).HasMaxLength(200).IsRequired();
            e.Property(r => r.RoomNumber).HasMaxLength(20).IsRequired();
            e.HasOne(r => r.ExternalExperience)
                .WithMany(x => x.Requests)
                .HasForeignKey(r => r.ExternalExperienceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ConciergeKnowledgeEntry>(e =>
        {
            e.Property(k => k.Title).HasMaxLength(200).IsRequired();
            e.HasOne(k => k.Hotel)
                .WithMany(h => h.ConciergeKnowledgeEntries)
                .HasForeignKey(k => k.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
