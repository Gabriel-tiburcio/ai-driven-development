using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelsController(IHotelService hotelService) : ControllerBase
{
    /// <summary>Resolves a hotel from the code embedded in the guest's QR-code URL (e.g. /h/{code}).</summary>
    [HttpGet("by-code/{code}")]
    public async Task<ActionResult<HotelDto>> GetByCode(string code, CancellationToken ct)
    {
        var hotel = await hotelService.GetByCodeAsync(code, ct);
        return hotel is null ? NotFound() : Ok(hotel);
    }
}
