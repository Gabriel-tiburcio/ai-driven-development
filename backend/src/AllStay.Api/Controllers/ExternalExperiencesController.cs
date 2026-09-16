using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/external-experiences")]
public class ExternalExperiencesController(IExternalExperienceService experienceService) : HotelScopedControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ExternalExperienceDto>>> List(Guid hotelId, CancellationToken ct)
        => Ok(await experienceService.ListForHotelAsync(hotelId, ct));

    [HttpGet("{experienceId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ExternalExperienceDto>> Get(Guid hotelId, Guid experienceId, CancellationToken ct)
    {
        var experience = await experienceService.GetAsync(hotelId, experienceId, ct);
        return experience is null ? NotFound() : Ok(experience);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ExternalExperienceDto>> Create(Guid hotelId, CreateExternalExperienceRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var created = await experienceService.CreateAsync(hotelId, request, ct);
        return CreatedAtAction(nameof(Get), new { hotelId, experienceId = created.Id }, created);
    }

    [HttpPut("{experienceId:guid}")]
    [Authorize]
    public async Task<ActionResult<ExternalExperienceDto>> Update(Guid hotelId, Guid experienceId, UpdateExternalExperienceRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var updated = await experienceService.UpdateAsync(hotelId, experienceId, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{experienceId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid hotelId, Guid experienceId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var deleted = await experienceService.DeleteAsync(hotelId, experienceId, ct);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{experienceId:guid}/requests")]
    [AllowAnonymous]
    public async Task<ActionResult<ExperienceRequestDto>> RequestExperience(Guid hotelId, Guid experienceId, CreateExperienceHireRequest request, CancellationToken ct)
    {
        var created = await experienceService.RequestAsync(hotelId, experienceId, request, ct);
        return created is null ? NotFound() : Ok(created);
    }

    /// <summary>Backoffice view of all booking requests for the hotel — staff only.</summary>
    [HttpGet("requests")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<ExperienceRequestDto>>> ListRequests(Guid hotelId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        return Ok(await experienceService.ListRequestsForHotelAsync(hotelId, ct));
    }
}
