using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

/// <summary>
/// Staff-only content the hotel feeds in to later "train" the Concierge Premium AI agent.
/// No guest-facing endpoint here — the guest app's Concierge screen is still a static visual mock.
/// </summary>
[ApiController]
[Route("api/hotels/{hotelId:guid}/concierge-knowledge")]
[Authorize]
public class ConciergeKnowledgeController(IConciergeKnowledgeService conciergeKnowledgeService) : HotelScopedControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConciergeKnowledgeEntryDto>>> List(Guid hotelId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        return Ok(await conciergeKnowledgeService.ListForHotelAsync(hotelId, ct));
    }

    [HttpPost]
    public async Task<ActionResult<ConciergeKnowledgeEntryDto>> Create(Guid hotelId, CreateConciergeKnowledgeEntryRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var created = await conciergeKnowledgeService.CreateAsync(hotelId, request, ct);
        return Ok(created);
    }

    [HttpPut("{entryId:guid}")]
    public async Task<ActionResult<ConciergeKnowledgeEntryDto>> Update(Guid hotelId, Guid entryId, UpdateConciergeKnowledgeEntryRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var updated = await conciergeKnowledgeService.UpdateAsync(hotelId, entryId, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{entryId:guid}")]
    public async Task<IActionResult> Delete(Guid hotelId, Guid entryId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var deleted = await conciergeKnowledgeService.DeleteAsync(hotelId, entryId, ct);
        return deleted ? NoContent() : NotFound();
    }
}
