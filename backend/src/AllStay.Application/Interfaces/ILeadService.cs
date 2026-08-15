using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface ILeadService
{
    Task<LeadDto> CreateAsync(CreateLeadRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<LeadDto>> ListAsync(CancellationToken ct = default);
}
