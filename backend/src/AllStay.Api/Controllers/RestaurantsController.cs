using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/restaurants")]
public class RestaurantsController(IRestaurantService restaurantService) : HotelScopedControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<RestaurantDto>>> List(Guid hotelId, CancellationToken ct)
        => Ok(await restaurantService.ListForHotelAsync(hotelId, ct));

    [HttpGet("{restaurantId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<RestaurantDto>> Get(Guid hotelId, Guid restaurantId, CancellationToken ct)
    {
        var restaurant = await restaurantService.GetAsync(hotelId, restaurantId, ct);
        return restaurant is null ? NotFound() : Ok(restaurant);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RestaurantDto>> Create(Guid hotelId, CreateRestaurantRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var created = await restaurantService.CreateAsync(hotelId, request, ct);
        return CreatedAtAction(nameof(Get), new { hotelId, restaurantId = created.Id }, created);
    }

    [HttpPut("{restaurantId:guid}")]
    [Authorize]
    public async Task<ActionResult<RestaurantDto>> Update(Guid hotelId, Guid restaurantId, UpdateRestaurantRequest request, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var updated = await restaurantService.UpdateAsync(hotelId, restaurantId, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{restaurantId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid hotelId, Guid restaurantId, CancellationToken ct)
    {
        if (!TryAuthorizeForHotel(hotelId, out var forbid)) return forbid!;
        var deleted = await restaurantService.DeleteAsync(hotelId, restaurantId, ct);
        return deleted ? NoContent() : NotFound();
    }
}
