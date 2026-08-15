using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

/// <summary>Hotel onboarding — creating hotels and their first staff login. No auth gate — open.</summary>
[ApiController]
[Route("api/admin")]
[AllowAnonymous]
public class AdminController(IHotelService hotelService) : ControllerBase
{
    [HttpGet("hotels")]
    public async Task<ActionResult<IReadOnlyList<HotelDto>>> ListHotels(CancellationToken ct)
        => Ok(await hotelService.ListAsync(ct));

    [HttpPost("hotels")]
    public async Task<ActionResult<HotelDto>> CreateHotel(CreateHotelRequest request, CancellationToken ct)
        => Ok(await hotelService.CreateAsync(request, ct));

    [HttpPost("hotels/{hotelId:guid}/staff")]
    public async Task<ActionResult<StaffProfileDto>> CreateStaff(Guid hotelId, CreateStaffRequest request, CancellationToken ct)
    {
        var staff = await hotelService.CreateStaffAsync(hotelId, request, ct);
        return staff is null ? NotFound() : Ok(staff);
    }
}
