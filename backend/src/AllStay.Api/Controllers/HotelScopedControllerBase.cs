using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

/// <summary>
/// Base for staff-only endpoints nested under /api/hotels/{hotelId}/...
/// Ensures the authenticated staff user's JWT "hotel_id" claim matches the route's hotelId,
/// so one hotel's staff can never act on another hotel's data.
/// </summary>
public abstract class HotelScopedControllerBase : ControllerBase
{
    protected bool TryAuthorizeForHotel(Guid hotelId, out ActionResult? forbidResult)
    {
        var claim = User.FindFirst("hotel_id")?.Value;
        if (claim is null || !Guid.TryParse(claim, out var claimHotelId) || claimHotelId != hotelId)
        {
            forbidResult = Forbid();
            return false;
        }

        forbidResult = null;
        return true;
    }
}
