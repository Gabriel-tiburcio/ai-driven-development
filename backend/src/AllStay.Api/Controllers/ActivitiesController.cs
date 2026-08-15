using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/activities")]
public class ActivitiesController(IActivityService activityService) : HotelScopedControllerBase
{
    /// <summary>Public catalog browsing — used by the guest PWA, no auth required.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ActivityDto>>> List(Guid hotelId, [FromQuery] string? category, CancellationToken ct)
        => Ok(await activityService.ListForHotelAsync(hotelId, category, ct));

    [HttpGet("{activityId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ActivityDto>> Get(Guid hotelId, Guid activityId, CancellationToken ct)
    {
        var activity = await activityService.GetAsync(hotelId, activityId, ct);
        return activity is null ? NotFound() : Ok(activity);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ActivityDto>> Create(Guid hotelId, CreateActivityRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var created = await activityService.CreateAsync(hotelId, request, ct);
        return CreatedAtAction(nameof(Get), new { hotelId, activityId = created.Id }, created);
    }

    [HttpPut("{activityId:guid}")]
    [Authorize]
    public async Task<ActionResult<ActivityDto>> Update(Guid hotelId, Guid activityId, UpdateActivityRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var updated = await activityService.UpdateAsync(hotelId, activityId, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{activityId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid hotelId, Guid activityId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var deleted = await activityService.DeleteAsync(hotelId, activityId, ct);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{activityId:guid}/slots")]
    [Authorize]
    public async Task<ActionResult<ActivitySlotDto>> AddSlot(Guid hotelId, Guid activityId, CreateActivitySlotRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var slot = await activityService.AddSlotAsync(hotelId, activityId, request, ct);
        return slot is null ? NotFound() : Ok(slot);
    }

    [HttpDelete("{activityId:guid}/slots/{slotId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteSlot(Guid hotelId, Guid activityId, Guid slotId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var deleted = await activityService.DeleteSlotAsync(hotelId, activityId, slotId, ct);
        return deleted ? NoContent() : NotFound();
    }
}
