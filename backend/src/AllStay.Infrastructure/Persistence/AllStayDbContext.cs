using AllStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Persistence;

public class AllStayDbContext(DbContextOptions<AllStayDbContext> options) : DbContext(options)
{
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<HotelStaffUser> HotelStaffUsers => Set<HotelStaffUser>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<ActivitySlot> ActivitySlots => Set<ActivitySlot>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Lead> Leads => Set<Lead>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hotel>(e =>
        {
            e.HasIndex(h => h.Code).IsUnique();
            e.Property(h => h.Name).HasMaxLength(200).IsRequired();
            e.Property(h => h.Code).HasMaxLength(50).IsRequired();
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
    }
}
