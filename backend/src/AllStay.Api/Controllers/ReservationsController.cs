using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/reservations")]
public class ReservationsController(IReservationService reservationService) : HotelScopedControllerBase
{
    /// <summary>Backoffice view of all reservations for the hotel — staff only.</summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<ReservationDto>>> ListForHotel(Guid hotelId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        return Ok(await reservationService.ListForHotelAsync(hotelId, ct));
    }

    /// <summary>Guest's own reservations, scoped by room number — no auth (guest never logs in).</summary>
    [HttpGet("mine")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ReservationDto>>> ListForGuest(Guid hotelId, [FromQuery] string room, CancellationToken ct)
        => Ok(await reservationService.ListForGuestAsync(hotelId, room, ct));

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ReservationDto>> Create(Guid hotelId, CreateReservationRequest request, CancellationToken ct)
    {
        try
        {
            var created = await reservationService.CreateAsync(hotelId, request, ct);
            return created is null ? NotFound() : Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{reservationId:guid}/cancel")]
    [AllowAnonymous]
    public async Task<IActionResult> Cancel(Guid hotelId, Guid reservationId, CancellationToken ct)
    {
        var cancelled = await reservationService.CancelAsync(hotelId, reservationId, ct);
        return cancelled ? NoContent() : NotFound();
    }
}
