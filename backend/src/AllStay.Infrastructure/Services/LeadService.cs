using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Domain.Entities;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllStay.Infrastructure.Services;

public class LeadService(AllStayDbContext db) : ILeadService
{
    public async Task<LeadDto> CreateAsync(CreateLeadRequest request, CancellationToken ct = default)
    {
        var lead = new Lead
        {
            HotelName = request.HotelName,
            ContactName = request.ContactName,
            Email = request.Email,
            Phone = request.Phone,
            Message = request.Message
        };

        db.Leads.Add(lead);
        await db.SaveChangesAsync(ct);

        return ToDto(lead);
    }

    public async Task<IReadOnlyList<LeadDto>> ListAsync(CancellationToken ct = default)
    {
        return await db.Leads
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new LeadDto(l.Id, l.HotelName, l.ContactName, l.Email, l.Phone, l.Message, l.CreatedAt))
            .ToListAsync(ct);
    }

    private static LeadDto ToDto(Lead l) => new(l.Id, l.HotelName, l.ContactName, l.Email, l.Phone, l.Message, l.CreatedAt);
}
