using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/events")]
public class EventsController(IEventService eventService) : HotelScopedControllerBase
{
    /// <summary>Public event listing — used by the guest PWA, no auth required.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> List(Guid hotelId, CancellationToken ct)
        => Ok(await eventService.ListForHotelAsync(hotelId, ct));

    [HttpGet("{eventId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<EventDto>> Get(Guid hotelId, Guid eventId, CancellationToken ct)
    {
        var evt = await eventService.GetAsync(hotelId, eventId, ct);
        return evt is null ? NotFound() : Ok(evt);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<EventDto>> Create(Guid hotelId, CreateEventRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var created = await eventService.CreateAsync(hotelId, request, ct);
        return CreatedAtAction(nameof(Get), new { hotelId, eventId = created.Id }, created);
    }

    [HttpPut("{eventId:guid}")]
    [Authorize]
    public async Task<ActionResult<EventDto>> Update(Guid hotelId, Guid eventId, UpdateEventRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var updated = await eventService.UpdateAsync(hotelId, eventId, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{eventId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid hotelId, Guid eventId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var deleted = await eventService.DeleteAsync(hotelId, eventId, ct);
        return deleted ? NoContent() : NotFound();
    }
}
