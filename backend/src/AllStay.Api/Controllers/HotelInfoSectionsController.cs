using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/info-sections")]
public class HotelInfoSectionsController(IHotelInfoSectionService sectionService) : HotelScopedControllerBase
{
    /// <summary>Public info sections — used by the guest PWA, no auth required.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<HotelInfoSectionDto>>> List(Guid hotelId, CancellationToken ct)
        => Ok(await sectionService.ListForHotelAsync(hotelId, ct));

    [HttpGet("{sectionId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<HotelInfoSectionDto>> Get(Guid hotelId, Guid sectionId, CancellationToken ct)
    {
        var section = await sectionService.GetAsync(hotelId, sectionId, ct);
        return section is null ? NotFound() : Ok(section);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<HotelInfoSectionDto>> Create(Guid hotelId, CreateHotelInfoSectionRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var created = await sectionService.CreateAsync(hotelId, request, ct);
        return CreatedAtAction(nameof(Get), new { hotelId, sectionId = created.Id }, created);
    }

    [HttpPut("{sectionId:guid}")]
    [Authorize]
    public async Task<ActionResult<HotelInfoSectionDto>> Update(Guid hotelId, Guid sectionId, UpdateHotelInfoSectionRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var updated = await sectionService.UpdateAsync(hotelId, sectionId, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{sectionId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid hotelId, Guid sectionId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var deleted = await sectionService.DeleteAsync(hotelId, sectionId, ct);
        return deleted ? NoContent() : NotFound();
    }
}
