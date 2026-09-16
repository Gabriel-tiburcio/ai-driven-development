using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class ServiceService(AllStayDbContext db) : IServiceService
{
    public async Task<IReadOnlyList<ServiceDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var services = await db.Services
            .Where(s => s.HotelId == hotelId && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);

        return services.Select(ToDto).ToList();
    }

    public async Task<ServiceDto?> GetAsync(Guid hotelId, Guid serviceId, CancellationToken ct = default)
    {
        var service = await db.Services.FirstOrDefaultAsync(s => s.HotelId == hotelId && s.Id == serviceId, ct);
        return service is null ? null : ToDto(service);
    }

    public async Task<ServiceDto> CreateAsync(Guid hotelId, CreateServiceItemRequest request, CancellationToken ct = default)
    {
        var service = new Service
        {
            HotelId = hotelId,
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Price = request.Price,
            DurationMinutes = request.DurationMinutes,
            ImageUrl = request.ImageUrl
        };

        db.Services.Add(service);
        await db.SaveChangesAsync(ct);
        return ToDto(service);
    }

    public async Task<ServiceDto?> UpdateAsync(Guid hotelId, Guid serviceId, UpdateServiceItemRequest request, CancellationToken ct = default)
    {
        var service = await db.Services.FirstOrDefaultAsync(s => s.HotelId == hotelId && s.Id == serviceId, ct);
        if (service is null) return null;

        service.Name = request.Name;
        service.Description = request.Description;
        service.Category = request.Category;
        service.Price = request.Price;
        service.DurationMinutes = request.DurationMinutes;
        service.ImageUrl = request.ImageUrl;
        service.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return ToDto(service);
    }

    public async Task<bool> DeleteAsync(Guid hotelId, Guid serviceId, CancellationToken ct = default)
    {
        var service = await db.Services.FirstOrDefaultAsync(s => s.HotelId == hotelId && s.Id == serviceId, ct);
        if (service is null) return false;

        db.Services.Remove(service);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<ServiceRequestDto?> RequestAsync(Guid hotelId, Guid serviceId, CreateServiceHireRequest request, CancellationToken ct = default)
    {
        var service = await db.Services.FirstOrDefaultAsync(s => s.HotelId == hotelId && s.Id == serviceId, ct);
        if (service is null) return null;

        var serviceRequest = new ServiceRequest
        {
            ServiceId = serviceId,
            GuestName = request.GuestName,
            RoomNumber = request.RoomNumber
        };

        db.ServiceRequests.Add(serviceRequest);
        await db.SaveChangesAsync(ct);

        return new ServiceRequestDto(
            serviceRequest.Id,
            serviceRequest.ServiceId,
            service.Name,
            serviceRequest.GuestName,
            serviceRequest.RoomNumber,
            serviceRequest.Status.ToString(),
            serviceRequest.CreatedAt);
    }

    public async Task<IReadOnlyList<ServiceRequestDto>> ListRequestsForHotelAsync(Guid hotelId, CancellationToken ct = default)
    {
        var requests = await db.ServiceRequests
            .Include(r => r.Service)
            .Where(r => r.Service!.HotelId == hotelId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return requests.Select(r => new ServiceRequestDto(
            r.Id,
            r.ServiceId,
            r.Service!.Name,
            r.GuestName,
            r.RoomNumber,
            r.Status.ToString(),
            r.CreatedAt)).ToList();
    }

    private static ServiceDto ToDto(Service s) => new(
        s.Id,
        s.HotelId,
        s.Name,
        s.Description,
        s.Category,
        s.Price,
        s.DurationMinutes,
        s.ImageUrl,
        s.IsActive);
}
