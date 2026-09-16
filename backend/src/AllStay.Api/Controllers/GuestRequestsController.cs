using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/requests")]
public class GuestRequestsController(IGuestRequestService requestService) : HotelScopedControllerBase
{
    /// <summary>Backoffice view of all guest requests for the hotel — staff only.</summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<GuestRequestDto>>> ListForHotel(Guid hotelId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        return Ok(await requestService.ListForHotelAsync(hotelId, ct));
    }

    /// <summary>Guest's own requests, scoped by room number — no auth (guest never logs in).</summary>
    [HttpGet("mine")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<GuestRequestDto>>> ListForGuest(Guid hotelId, [FromQuery] string room, CancellationToken ct)
        => Ok(await requestService.ListForGuestAsync(hotelId, room, ct));

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<GuestRequestDto>> Create(Guid hotelId, CreateGuestRequestRequest request, CancellationToken ct)
        => Ok(await requestService.CreateAsync(hotelId, request, ct));

    [HttpPatch("{requestId:guid}/status")]
    [Authorize]
    public async Task<ActionResult<GuestRequestDto>> UpdateStatus(Guid hotelId, Guid requestId, UpdateGuestRequestStatusRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var updated = await requestService.UpdateStatusAsync(hotelId, requestId, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }
}
