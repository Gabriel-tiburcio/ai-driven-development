using AllStay.Api.Auth;
using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

/// <summary>Hotel onboarding and access management — creating/enabling hotels and their staff logins. Gated by shared admin key.</summary>
[ApiController]
[Route("api/admin")]
[AdminApiKey]
public class AdminController(IHotelService hotelService) : ControllerBase
{
    [HttpGet("hotels")]
    public async Task<ActionResult<IReadOnlyList<HotelDto>>> ListHotels(CancellationToken ct)
        => Ok(await hotelService.ListAsync(ct));

    [HttpPost("hotels")]
    public async Task<ActionResult<HotelDto>> CreateHotel(CreateHotelRequest request, CancellationToken ct)
        => Ok(await hotelService.CreateAsync(request, ct));

    [HttpPatch("hotels/{hotelId:guid}/status")]
    public async Task<IActionResult> SetHotelStatus(Guid hotelId, UpdateActiveStatusRequest request, CancellationToken ct)
        => await hotelService.SetHotelActiveAsync(hotelId, request.IsActive, ct) ? NoContent() : NotFound();

    [HttpGet("hotels/{hotelId:guid}/staff")]
    public async Task<ActionResult<IReadOnlyList<HotelStaffDto>>> ListStaff(Guid hotelId, CancellationToken ct)
        => Ok(await hotelService.ListStaffAsync(hotelId, ct));

    [HttpPost("hotels/{hotelId:guid}/staff")]
    public async Task<ActionResult<StaffProfileDto>> CreateStaff(Guid hotelId, CreateStaffRequest request, CancellationToken ct)
    {
        var staff = await hotelService.CreateStaffAsync(hotelId, request, ct);
        return staff is null ? NotFound() : Ok(staff);
    }

    [HttpPatch("hotels/{hotelId:guid}/staff/{staffId:guid}/status")]
    public async Task<IActionResult> SetStaffStatus(Guid hotelId, Guid staffId, UpdateActiveStatusRequest request, CancellationToken ct)
        => await hotelService.SetStaffActiveAsync(hotelId, staffId, request.IsActive, ct) ? NoContent() : NotFound();
}
