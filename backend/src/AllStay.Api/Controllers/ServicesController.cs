using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/services")]
public class ServicesController(IServiceService serviceService) : HotelScopedControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ServiceDto>>> List(Guid hotelId, CancellationToken ct)
        => Ok(await serviceService.ListForHotelAsync(hotelId, ct));

    [HttpGet("{serviceId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceDto>> Get(Guid hotelId, Guid serviceId, CancellationToken ct)
    {
        var service = await serviceService.GetAsync(hotelId, serviceId, ct);
        return service is null ? NotFound() : Ok(service);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ServiceDto>> Create(Guid hotelId, CreateServiceItemRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var created = await serviceService.CreateAsync(hotelId, request, ct);
        return CreatedAtAction(nameof(Get), new { hotelId, serviceId = created.Id }, created);
    }

    [HttpPut("{serviceId:guid}")]
    [Authorize]
    public async Task<ActionResult<ServiceDto>> Update(Guid hotelId, Guid serviceId, UpdateServiceItemRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var updated = await serviceService.UpdateAsync(hotelId, serviceId, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{serviceId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid hotelId, Guid serviceId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var deleted = await serviceService.DeleteAsync(hotelId, serviceId, ct);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{serviceId:guid}/requests")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceRequestDto>> RequestService(Guid hotelId, Guid serviceId, CreateServiceHireRequest request, CancellationToken ct)
    {
        var created = await serviceService.RequestAsync(hotelId, serviceId, request, ct);
        return created is null ? NotFound() : Ok(created);
    }

    /// <summary>Backoffice view of all hire requests for the hotel — staff only.</summary>
    [HttpGet("requests")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<ServiceRequestDto>>> ListRequests(Guid hotelId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        return Ok(await serviceService.ListRequestsForHotelAsync(hotelId, ct));
    }
}
