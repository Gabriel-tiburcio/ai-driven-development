using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/kids-activities")]
public class KidsActivitiesController(IKidsActivityService kidsActivityService) : HotelScopedControllerBase
{
    /// <summary>Public catalog browsing — used by the guest PWA, no auth required.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<KidsActivityDto>>> List(Guid hotelId, CancellationToken ct)
        => Ok(await kidsActivityService.ListForHotelAsync(hotelId, ct));

    [HttpGet("{kidsActivityId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<KidsActivityDto>> Get(Guid hotelId, Guid kidsActivityId, CancellationToken ct)
    {
        var activity = await kidsActivityService.GetAsync(hotelId, kidsActivityId, ct);
        return activity is null ? NotFound() : Ok(activity);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<KidsActivityDto>> Create(Guid hotelId, CreateKidsActivityRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var created = await kidsActivityService.CreateAsync(hotelId, request, ct);
        return CreatedAtAction(nameof(Get), new { hotelId, kidsActivityId = created.Id }, created);
    }

    [HttpPut("{kidsActivityId:guid}")]
    [Authorize]
    public async Task<ActionResult<KidsActivityDto>> Update(Guid hotelId, Guid kidsActivityId, UpdateKidsActivityRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var updated = await kidsActivityService.UpdateAsync(hotelId, kidsActivityId, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{kidsActivityId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid hotelId, Guid kidsActivityId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var deleted = await kidsActivityService.DeleteAsync(hotelId, kidsActivityId, ct);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{kidsActivityId:guid}/enrollments")]
    [AllowAnonymous]
    public async Task<ActionResult<KidsEnrollmentDto>> Enroll(Guid hotelId, Guid kidsActivityId, CreateKidsEnrollmentRequest request, CancellationToken ct)
    {
        var enrollment = await kidsActivityService.EnrollAsync(hotelId, kidsActivityId, request, ct);
        return enrollment is null ? NotFound() : Ok(enrollment);
    }

    /// <summary>Backoffice view of all kids-activity enrollments for the hotel — staff only.</summary>
    [HttpGet("enrollments")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<KidsEnrollmentDto>>> ListEnrollments(Guid hotelId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        return Ok(await kidsActivityService.ListEnrollmentsForHotelAsync(hotelId, ct));
    }
}
